using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using MEC;

namespace SCP
{
	public class EventHandler : IEventHandlerWaitingForPlayers, IEventHandlerPlayerJoin, IEventHandlerRoundStart, IEventHandlerRoundRestart, IEventHandlerRoundEnd, IEventHandlerCheckRoundEnd
	{
		private readonly SCP plugin;
		public EventHandler(SCP plugin) => this.plugin = plugin;

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
					plugin.Server.Map.ClearBroadcasts();
					plugin.Server.Map.Broadcast(15, "<color=#c50000>Mystery SCP</color> gamemode is starting..", false);
				}
			}
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{

				plugin.RoundStarted = true;

				foreach (Player player in ev.Server.GetPlayers())
					if (player.TeamRole.Team == Smod2.API.Team.SCP)
						Timing.RunCoroutine(plugin.Functions.SpawnSCP(player));
					else
					{
						player.PersonalClearBroadcasts();
						player.PersonalBroadcast(15, "You are a human. All the <color=#c50000>SCP's</color> are the same type. There are currently " + ev.Server.Round.Stats.SCPAlive + " SCP's. Good luck.", false);
					}
			}
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

			bool scpAlive = false;
			bool humanAlive = false;

			foreach (Player player in ev.Server.GetPlayers())
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					scpAlive = true; continue;
				}
				else if (player.TeamRole.Team != Smod2.API.Team.SCP && player.TeamRole.Team != Smod2.API.Team.SPECTATOR && player.TeamRole.Team != Smod2.API.Team.TUTORIAL)
					humanAlive = true;

			if (ev.Server.GetPlayers().Count > 1)
				if (scpAlive && humanAlive)
					ev.Status = ROUND_END_STATUS.ON_GOING;
				else if (scpAlive && !humanAlive)
				{
					ev.Status = ROUND_END_STATUS.SCP_VICTORY;
					plugin.Functions.EndGamemodeRound();
				}
				else if (!scpAlive && humanAlive)
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY;
					plugin.Functions.EndGamemodeRound();
				}

		}
	}
}