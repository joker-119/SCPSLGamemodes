using System.Collections.Generic;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace MassacreGamemode
{
	internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd,
		IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie, IEventHandlerRoundRestart
	{
		private readonly Massacre plugin;

		public EventsHandler(Massacre plugin) => this.plugin = plugin;
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;
			if (plugin.RoundStarted) return;
			
			Server server = plugin.Server;
			server.Map.ClearBroadcasts();
			server.Map.Broadcast(25, "<color=#50c878>Massacre Gamemode</color> is starting...", false);
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;
			plugin.RoundStarted = true;
			List<Player> players = ev.Server.GetPlayers();

			foreach (Door door in ev.Server.Map.GetDoors())
			{
				door.Locked = true;
				door.Open = false;
			}

			plugin.Server.Map.ClearBroadcasts();
			plugin.Info("Massacre of the D-Bois Gamemode Started!");

			for (int i = 0; i < plugin.NutCount; i++)
			{
				int random = plugin.Gen.Next(players.Count);
				Player player = players[random];
				players.Remove(player);
				Timing.RunCoroutine(plugin.Functions.SpawnNut(player));
			}

			foreach (Player player in players) Timing.RunCoroutine(plugin.Functions.SpawnDboi(player));
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Ended!");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Restarted.");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.TeamRole.Role != Role.CLASSD) return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(5, "There are now " + (plugin.Server.Round.Stats.ClassDAlive - 1) + " Class-D remaining.", false);
			ev.Player.PersonalBroadcast(25, "You are dead! But don't worry, now you get to relax and watch your friends die!", false);
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool peanutAlive = false;
			bool humanAlive = false;
			int humanCount = 0;

			foreach (Player player in ev.Server.GetPlayers())
				if (player.TeamRole.Team == Team.SCP)
					peanutAlive = true;

				else if (player.TeamRole.Team != Team.SCP && player.TeamRole.Team != Team.SPECTATOR)
				{
					humanAlive = true;
					humanCount++;
				}

			if (ev.Server.GetPlayers().Count <= 1) return;

			if (peanutAlive && humanAlive && humanCount > 1)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (peanutAlive && humanAlive && humanCount == 1)
			{
				ev.Status = ROUND_END_STATUS.OTHER_VICTORY;
				plugin.Functions.EndGamemodeRound();
				foreach (Player player in ev.Server.GetPlayers())
					if (player.TeamRole.Team == Team.CLASSD)
					{
						ev.Server.Map.ClearBroadcasts();
						ev.Server.Map.Broadcast(10, player.Name + " Winner, winner, chicken dinner!", false);
					}
			}
			else if (peanutAlive && humanAlive == false)
			{
				ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
			}
			else if (peanutAlive == false && humanAlive)
			{
				ev.Status = ROUND_END_STATUS.CI_VICTORY; plugin.Functions.EndGamemodeRound();
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;
			plugin.Info("Mass Respawn.");

			ev.SpawnChaos = true;
			ev.PlayerList = new List<Player>();
		}
	}
}
