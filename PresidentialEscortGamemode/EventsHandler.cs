using System;
using System.Collections.Generic;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace PresidentialEscortGamemode
{
	internal class EventsHandler : IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerRoundRestart,
		 IEventHandlerCheckEscape, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie, IEventHandlerTeamRespawn
	{
		private readonly PresidentialEscort plugin;

		public EventsHandler(PresidentialEscort plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled) return;
			if (plugin.RoundStarted) return;
			
			Server server = plugin.Server;
			server.Map.ClearBroadcasts();
			server.Map.Broadcast(25, "<color=#f8ea56>Presidential Escort</color> gamemode is starting...", false);
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) return;
			
			plugin.RoundStarted = true;
			List<Player> players = ev.Server.GetPlayers();

			plugin.Server.Map.ClearBroadcasts();
			plugin.Info("Presidential Escort Gamemode Started!");

			// chooses and spawns VIP scientist
			Player vip;
			if (plugin.Vip == null)
			{
				int chosenVip = new Random().Next(players.Count);
				vip = players[chosenVip];
			}
			else
				vip = plugin.Vip;

			plugin.Info("" + vip.Name + " chosen as the VIP");

			Timing.RunCoroutine(plugin.Functions.SpawnVip(vip));
			players.Remove(vip);

			// spawn NTF into round
			foreach (Player player in players)
				if (player.TeamRole.Team != Smod2.API.Team.SCP)
					Timing.RunCoroutine(plugin.Functions.SpawnNtf(player));

			Timing.RunCoroutine(plugin.Functions.AnnounceLocation(120f));
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.SteamId == plugin.Vip.SteamId)
				plugin.Vip = null;
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

		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.SteamId == plugin.Vip.SteamId)
				plugin.VipEscaped = true;
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool vipAlive = false;
			bool scpAlive = false;

			if (plugin.Vip == null)
			{
				plugin.Info("VIP not found. Ending gamemode.");
				ev.Status = ROUND_END_STATUS.NO_VICTORY;
				plugin.Functions.EndGamemodeRound();

				return;
			}

			foreach (Player player in ev.Server.GetPlayers())
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
					scpAlive = true;
				else if (player.SteamId == plugin.Vip.SteamId) vipAlive = true;

			if (ev.Server.GetPlayers().Count <= 1) return;
			
			if (plugin.VipEscaped || vipAlive && !scpAlive)
			{
				ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemodeRound();
			}
			else if (vipAlive && scpAlive)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (scpAlive && !vipAlive)
			{
				ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("President Respawn.");

			ev.SpawnChaos = false;
		}
	}
}
