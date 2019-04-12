using System.Data;
using System.Runtime.CompilerServices;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using MEC;
using UnityEngine;
using System.Linq;

namespace PresidentialEscortGamemode
{
	internal class EventsHandler : IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerRoundRestart,
		 IEventHandlerCheckEscape, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie
	{
		private readonly PresidentialEscort plugin;

		public EventsHandler(PresidentialEscort plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				if (!plugin.RoundStarted)
				{
					Server server = plugin.Server;
					server.Map.ClearBroadcasts();
					server.Map.Broadcast(25, "<color=#f8ea56>Presidential Escort</color> gamemode is starting...", false);
				}
			}
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				plugin.RoundStarted = true;
				List<Player> players = ev.Server.GetPlayers();

				plugin.Server.Map.ClearBroadcasts();
				plugin.Info("Presidential Escort Gamemode Started!");

				// chooses and spawns VIP scientist
				Player vip;
				if (!(plugin.VIP is Player))
				{
					int chosenVIP = new System.Random().Next(players.Count);
					vip = players[chosenVIP];
				}
				else
					vip = plugin.VIP;

				plugin.Info("" + vip.Name + " chosen as the VIP");

				Timing.RunCoroutine(plugin.Functions.SpawnVIP(vip));
				players.Remove(vip);

				// spawn NTF into round
				foreach (Player player in players)
				{
					if (player.TeamRole.Team != Smod2.API.Team.SCP)
						Timing.RunCoroutine(plugin.Functions.SpawnNTF(player));
				}
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.SteamId == plugin.VIP.SteamId)
				plugin.VIP = null;
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

		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.SteamId == plugin.VIP.SteamId)
				plugin.VIPEscaped = true;
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool vipAlive = false;
			bool scpAlive = false;

			if (!(plugin.VIP is Player))
			{
				plugin.Info("VIP not found. Ending gamemode.");
				ev.Status = ROUND_END_STATUS.NO_VICTORY;
				plugin.Functions.EndGamemodeRound();

				return;
			}

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					scpAlive = true; continue;
				}
				else if (player.SteamId == plugin.VIP.SteamId)
				{
					vipAlive = true;
				}
			}

			if (ev.Server.GetPlayers().Count > 1)
			{
				if (plugin.VIPEscaped || (vipAlive && !scpAlive))
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemodeRound();
				}
				else if (vipAlive && scpAlive)
				{
					ev.Status = ROUND_END_STATUS.ON_GOING;
				}
				else if (scpAlive && !vipAlive)
				{
					ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
				}
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			plugin.Info("President Respawn.");

			ev.SpawnChaos = false;
		}
	}
}
