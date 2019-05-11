using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;

namespace SCPRouletteGamemode
{
	public class EventHandlers : IEventHandlerWaitingForPlayers, IEventHandlerPlayerJoin, IEventHandlerPlayerDie,
		IEventHandlerRoundStart, IEventHandlerRoundRestart, IEventHandlerRoundEnd
	{
		private readonly ScpRouletteGamemode plugin;
		public EventHandlers(ScpRouletteGamemode plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.Info("Hello.");
		}

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
			if (ev.Killer.TeamRole.Team == Smod2.API.Team.SCP)
				plugin.Functions.PickScp(ev.Killer);
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			plugin.RoundStarted = false;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			plugin.RoundStarted = false;
		}
	}
}