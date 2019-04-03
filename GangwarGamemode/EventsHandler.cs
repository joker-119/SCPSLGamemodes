using System.Linq;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using Smod2.Events;
using System.Collections.Generic;
using scp4aiur;

namespace Gangwar
{
	internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerPlayerJoin, IEventHandlerWaitingForPlayers
	{
		private readonly Gangwar plugin;

		public EventsHandler(Gangwar plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (plugin.Enabled && !plugin.RoundStarted)
			{
				Server server = plugin.pluginManager.Server;
				server.Map.ClearBroadcasts();
				server.Map.Broadcast(25, "<color=00FFFF> Gangwar Gamemode</color> is starting..", false);
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
				plugin.Server.Map.StartWarhead();
				plugin.pluginManager.Server.Map.ClearBroadcasts();
				plugin.Info("Gangwar Gamemode started!");
				List<Player> players = ev.Server.GetPlayers();
				int num = players.Count / 2;

				for (int i = 0; i < num; i++)
				{
					int random = plugin.Gen.Next(players.Count);
					Player randomplayer = players[random];
					players.Remove(randomplayer);
					Timing.Run(plugin.Functions.SpawnNTF(randomplayer, 0));
				}

				foreach (Player player in players)
				{
					if (player.TeamRole.Role != Role.NTF_COMMANDER)
						Timing.Run(plugin.Functions.SpawnChaos(player, 0));
				}
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			plugin.Info("Round Ended!");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			bool ciAlive = false;
			bool ntfAlive = false;

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Team.CHAOS_INSURGENCY)
				{
					ciAlive = true; continue;
				}
				else if (player.TeamRole.Team == Team.NINETAILFOX)
				{
					ntfAlive = true;
				}
			}

			if (ev.Server.GetPlayers().Count > 1)
			{
				if (ciAlive && ntfAlive)
				{
					ev.Status = ROUND_END_STATUS.ON_GOING;
					ev.Server.Map.ClearBroadcasts();
					ev.Server.Map.Broadcast(10, "There are " + plugin.Round.Stats.CiAlive + " Chaos alive, and " + plugin.Round.Stats.NTFAlive + " NTF alive.", false);
				}
				else if (ciAlive && ntfAlive == false)
				{
					ev.Status = ROUND_END_STATUS.OTHER_VICTORY; plugin.Functions.EndGamemodeRound();
				}
				else if (ciAlive == false && ntfAlive)
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemodeRound();
				}
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;
			plugin.Info("Gang Respawn.");

			if (plugin.Round.Stats.CiAlive >= plugin.Round.Stats.NTFAlive)
			{
				ev.SpawnChaos = false;
			}
			else if (plugin.Round.Stats.CiAlive < plugin.Round.Stats.NTFAlive)
			{
				ev.SpawnChaos = true;
			}
		}
	}
}