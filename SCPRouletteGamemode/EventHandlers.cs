using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;

namespace SCPRouletteGamemode
{
	public class EventHandlers : IEventHandlerPlayerJoin, IEventHandlerPlayerDie,
		IEventHandlerRoundStart, IEventHandlerRoundRestart, IEventHandlerRoundEnd
	{
		private readonly ScpRouletteGamemode plugin;
		public EventHandlers(ScpRouletteGamemode plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;
			if (plugin.RoundStarted) return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(15, "SCP Roulette Gamemode is starting..", false);
		}
		
		public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;

			plugin.RoundStarted = true;
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Killer.SteamId == ev.Player.SteamId || ev.DamageTypeVar == DamageType.LURE ||
			    ev.DamageTypeVar == DamageType.NUKE || ev.DamageTypeVar == DamageType.DECONT ||
			    ev.DamageTypeVar == DamageType.WALL || ev.DamageTypeVar == DamageType.CONTAIN) return;
			
			if (ev.Killer.TeamRole.Team == Smod2.API.Team.SCP)
				plugin.Functions.PickScp(ev.Killer);
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
	}
}