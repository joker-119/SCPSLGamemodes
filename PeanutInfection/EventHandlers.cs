using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using MEC;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;

namespace PeanutInfection
{
	public class EventHandlers : IEventHandlerWaitingForPlayers, IEventHandlerPlayerJoin, IEventHandlerRoundEnd,
		IEventHandlerRoundRestart, IEventHandlerRoundStart, IEventHandlerPlayerDie, IEventHandlerCheckEscape,
		IEventHandlerCheckRoundEnd, IEventHandlerTeamRespawn
	{
		private readonly PeanutInfection plugin;
		public EventHandlers(PeanutInfection plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.Enabled)
				return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(25, "<color=red>Peanut Infection</color> gamemode is starting...", false);
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted)
				return;
			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(25, "<color=red>Peanut Infection</color> gamemode is starting..", false);
			}
			else
			{
				ev.Player.PersonalBroadcast(10, "Now Playing: <color=red>Peanut Infection</color> gamemode.", false);
				Timing.RunCoroutine(plugin.Functions.SpawnDboi(ev.Player, 5));
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted)
				return;
			plugin.RoundStarted = false;
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted)
				return;
			plugin.RoundStarted = false;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled)
				return;
			plugin.RoundStarted = true;

			List<Player> players = plugin.Server.GetPlayers();

			foreach (Player player in players)
				Timing.RunCoroutine(player.TeamRole.Team == Smod2.API.Team.SCP
					? plugin.Functions.SpawnNut(player, 5)
					: plugin.Functions.SpawnDboi(player, 5));
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted)
				return;

			if (ev.Player.TeamRole.Team != Smod2.API.Team.SCP)
				Timing.RunCoroutine(plugin.RespawnOnKiller
					? plugin.Functions.SpawnNut(ev.Player, 5, ev.Killer.GetPosition())
					: plugin.Functions.SpawnNut(ev.Player, 5));
		}

		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (!plugin.RoundStarted)
				return;
			
			if (ev.AllowEscape)
				plugin.Server.Map.DetonateWarhead();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted)
				return;
			
			bool peanutAlive = false;
			bool classDAlive = false;
			
			foreach (Player player in plugin.Server.GetPlayers())
				switch (player.TeamRole.Role)
				{
					case Role.SCP_173:
						peanutAlive = true;
						break;
					case Role.CLASSD:
						classDAlive = true;
						break;
				}

			if (plugin.Server.GetPlayers().Count <= 1)
				return;

			if (peanutAlive && classDAlive)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (peanutAlive && !classDAlive)
			{
				ev.Status = ROUND_END_STATUS.SCP_VICTORY;
				plugin.Functions.EndGamemodeRound();
			}
			else if (!peanutAlive && classDAlive)
			{
				ev.Status = ROUND_END_STATUS.OTHER_VICTORY;
				plugin.Functions.EndGamemodeRound();
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted)
				return;
			plugin.Info("Peanut Infection respawn event");

			ev.SpawnChaos = true;
			ev.PlayerList = new List<Player>();
		}
	}
}