using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;

namespace RisingLavaGamemode
{
	public class EventHandlers : IEventHandlerWaitingForPlayers, IEventHandlerRoundStart, IEventHandlerRoundRestart,
		IEventHandlerRoundEnd, IEventHandlerCheckRoundEnd, IEventHandlerTeamRespawn, IEventHandlerPlayerJoin
	{
		private readonly RisingLavaGamemode plugin;
		public EventHandlers(RisingLavaGamemode plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;
			plugin.Functions.Get079Rooms();
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(25, "Rising Lava Gamemode is starting..", false);
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;

			plugin.RoundStarted = true;
			
			plugin.Server.Map.ClearBroadcasts();

			foreach (Player player in plugin.Server.GetPlayers())
			{
				player.ChangeRole(Role.CLASSD);
				plugin.Server.Map.Broadcast(10, "Rising Lava gamemode has started. You have " + plugin.LczDelay + " seconds until LCZ is locked down.", false);
			}

			Timing.RunCoroutine(plugin.Functions.LczLockdown(plugin.LczDelay));
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.RoundStarted = false;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.RoundStarted = false;
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (plugin.Server.Round.Stats.ClassDAlive > 1)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else
			{
				foreach (Player player in plugin.Server.GetPlayers().Where(ply => ply.TeamRole.Role == Role.CLASSD))
					plugin.Server.Map.Broadcast(25, player.Name + " has won!", false);
				ev.Status = ROUND_END_STATUS.CI_VICTORY;
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			ev.SpawnChaos = true;
			ev.PlayerList = new List<Player>();
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;
			if (plugin.RoundStarted) return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(15, "Hostage Situation gamemode is starting..", false);
		}
	}
}