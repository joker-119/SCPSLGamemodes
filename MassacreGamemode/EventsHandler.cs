using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using scp4aiur;

namespace MassacreGamemode
{
	internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd,
		IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie, IEventHandlerRoundRestart
	{
		private readonly Massacre plugin;

		public EventsHandler(Massacre plugin) => this.plugin = plugin;
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (plugin.Enabled)
			{
				if (!plugin.RoundStarted)
				{
					Server server = plugin.Server;
					server.Map.ClearBroadcasts();
					server.Map.Broadcast(25, "<color=#50c878>Massacre Gamemode</color> is starting...", false);
				}
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (plugin.Enabled)
			{
				plugin.RoundStarted = true;
				List<Player> players = ev.Server.GetPlayers();
				List<string> nuts = new List<string>();

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
					Player randomplayer = players[random];
					players.Remove(randomplayer);
					Timing.Run(plugin.Functions.SpawnNut(randomplayer, 0));
				}

				foreach (Player player in players)
				{
					Timing.Run(plugin.Functions.SpawnDboi(player, 0));
				}
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

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
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			if (ev.Player.TeamRole.Role == Role.CLASSD)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(5, "There are now " + (plugin.Server.Round.Stats.ClassDAlive - 1) + " Class-D remaining.", false);
				ev.Player.PersonalBroadcast(25, "You are dead! But don't worry, now you get to relax and watch your friends die!", false);
			}
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool peanutAlive = false;
			bool humanAlive = false;
			int humanCount = 0;

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Team.SCP)
				{
					peanutAlive = true; continue;
				}

				else if (player.TeamRole.Team != Team.SCP && player.TeamRole.Team != Team.SPECTATOR)
				{
					humanAlive = true;
					humanCount++;
				}

			}

			if (ev.Server.GetPlayers().Count > 1)
			{
				if (peanutAlive && humanAlive && humanCount > 1)
				{
					ev.Status = ROUND_END_STATUS.ON_GOING;
				}
				else if (peanutAlive && humanAlive && humanCount == 1)
				{
					ev.Status = ROUND_END_STATUS.OTHER_VICTORY;
					plugin.Functions.EndGamemodeRound();

					foreach (Player player in ev.Server.GetPlayers())
					{
						if (player.TeamRole.Team == Team.CLASSD)
						{
							ev.Server.Map.ClearBroadcasts();
							ev.Server.Map.Broadcast(10, player.Name + " Winner, winner, chicken dinner!", false);
							plugin.Winner = player;
						}
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
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;
			plugin.Info("Mass Respawn.");

			ev.SpawnChaos = true;
			ev.PlayerList = new List<Player>();
		}
	}
}
