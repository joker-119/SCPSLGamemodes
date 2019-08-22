using System.Collections.Generic;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace ThreeMusketeers
{
	internal class EventsHandler : IEventHandlerWaitingForPlayers, IEventHandlerRoundStart, IEventHandlerRoundRestart, IEventHandlerRoundEnd, IEventHandlerTeamRespawn, IEventHandlerPlayerJoin,
		 IEventHandlerPlayerDie, IEventHandlerCheckRoundEnd
	{
		private readonly Musketeers plugin;
		public EventsHandler(Musketeers plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled) return;

			if (plugin.RoundStarted) return;
			
			Server server = plugin.Server;
			server.Map.ClearBroadcasts();
			server.Map.Broadcast(25, "<color=#308ADA>Three Musketeers</color> gamemode is starting..", false);
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) return;
			
			plugin.RoundStarted = true;
			List<Player> players = ev.Server.GetPlayers();

			if (players.Count > 4)
			{
				for (int i = 0; i < 3; i++)
				{
					int random = plugin.Gen.Next(players.Count);
					Player randomPlayer = players[random];
					players.Remove(randomPlayer);
					Timing.RunCoroutine(plugin.Functions.SpawnNtf(randomPlayer));
				}

				foreach (Player player in players) Timing.RunCoroutine(plugin.Functions.SpawnClassD(player));
			}
			else
			{
				plugin.Error("You must have at least 4 players to play this gamemode.");
				plugin.Enabled = false;
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;
			plugin.Info("Musk Respawn.");

			ev.SpawnChaos = true;
			ev.PlayerList = new List<Player>();
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

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool muskyAlive = false;
			bool classDAlive = false;

			foreach (Player player in ev.Server.GetPlayers())
				switch (player.TeamRole.Team)
				{
					case Team.CLASSD:
					case Team.CHAOS_INSURGENCY:
						classDAlive = true;
						break;
					case Team.NINETAILFOX:
						muskyAlive = true;
						break;
				}

			if (ev.Server.GetPlayers().Count <= 1) return;
			
			if (classDAlive && muskyAlive)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (classDAlive && !muskyAlive)
			{
				ev.Status = ROUND_END_STATUS.OTHER_VICTORY;
				plugin.Functions.EndGamemodeRound();
			}
			else if (!classDAlive && muskyAlive)
			{
				ev.Status = ROUND_END_STATUS.MTF_VICTORY;
				plugin.Functions.EndGamemodeRound();
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Server.Map.ClearBroadcasts();
			if (ev.Player.TeamRole.Team == Team.NINETAILFOX)
				plugin.Server.Map.Broadcast(15, "There are now " + (plugin.Server.Round.Stats.NTFAlive - 1) + "<color=#308ADA> Musketeers alive!</color>", false);
			else if (ev.Player.TeamRole.Team != Team.NINETAILFOX) plugin.Server.Map.Broadcast(15, "There are now " + (plugin.Server.Round.Stats.ClassDAlive + plugin.Server.Round.Stats.CiAlive - 1) + "<color=#DAA130> Class-D alive!</color>", false);
		}
	}
}