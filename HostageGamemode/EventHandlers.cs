using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;

namespace HostageGamemode
{
	public class EventHandlers : IEventHandlerWaitingForPlayers, IEventHandlerRoundStart, IEventHandlerRoundRestart,
		IEventHandlerRoundEnd, IEventHandlerCheckRoundEnd, IEventHandlerCheckEscape, IEventHandlerTeamRespawn, IEventHandlerPlayerDie, IEventHandlerPlayerJoin
	{
		private readonly HostageGamemode plugin;
		public EventHandlers(HostageGamemode plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.Enabled) return;
			
			plugin.CriminalSpawn = ev.Server.Map.GetRandomSpawnPoint(Role.FACILITY_GUARD);
			plugin.PoliceSpawn = ev.Server.Map.GetRandomSpawnPoint(Role.FACILITY_GUARD);

			if (plugin.CriminalSpawn == plugin.PoliceSpawn)
				plugin.PoliceSpawn = ev.Server.Map.GetRandomSpawnPoint(Role.FACILITY_GUARD);
			
			ev.Server.Map.Broadcast(25, "<color=#123456>Hostage Situation gamemode is starting..</color>", false);
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) return;

			plugin.RoundStarted = true;

			foreach (Player player in ev.Server.GetPlayers())
				if (player.TeamRole.Role == Role.FACILITY_GUARD)
					Timing.RunCoroutine(plugin.Functions.SpawnPolice(player));
				else
					player.ChangeRole(Role.SPECTATOR);
			
			List<Player> players = ev.Server.GetPlayers().Where(ply => ply.TeamRole.Role == Role.SPECTATOR).ToList();

			for (int i = 0; i < plugin.HostageCount; i++)
			{
				if (players.Count < 3) break;
				int r = plugin.Gen.Next(players.Count);

				Timing.RunCoroutine(plugin.Functions.SpawnHostage(players[r]));
				
				players.RemoveAt(r);
				plugin.Hostages.Add(players[r].PlayerId);
			}

			for (int i = 0; i < plugin.CriminalCount; i++)
			{
				if (players.Count < 2) break;
				int r = plugin.Gen.Next(players.Count);

				Timing.RunCoroutine(plugin.Functions.SpawnCriminal(players[r]));
				
				players.RemoveAt(r);
			}

			foreach (Player player in players) 
				Timing.RunCoroutine(plugin.Functions.SpawnSwat(player));
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;
			
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;
			
			plugin.Functions.EndGamemodeRound();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;
			
			if (ev.Server.Round.Duration < 5)
			{
				ev.Status = ROUND_END_STATUS.ON_GOING;
				return;
			}

			if (plugin.Hostages.Count > 0)
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (!plugin.RoundStarted) return;
			
			if (plugin.Hostages.Contains(ev.Player.PlayerId)) 
				plugin.Hostages.Remove(ev.Player.PlayerId);
			
			if (plugin.Hostages.Count < 1)
				plugin.Functions.EndGamemodeRound();
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;
			
			ev.SpawnChaos = false;

			foreach (Player player in ev.PlayerList)
				Timing.RunCoroutine(plugin.Functions.SpawnSwat(player));
			
			ev.PlayerList = new List<Player>();
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;
			
			if (plugin.Hostages.Contains(ev.Player.PlayerId)) 
				plugin.Hostages.Remove(ev.Player.PlayerId);
			
			if (plugin.Hostages.Count < 1)
				plugin.Functions.EndGamemodeRound();
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled) return;
			if (plugin.RoundStarted) return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(25, "<color=#123456>Hostage Situation gamemode is starting..</color>", false);
		}
	}
}