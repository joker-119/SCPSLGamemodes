using MEC;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;

namespace FindersKeepersGamemode
{
	public class EventHandlers : IEventHandlerWaitingForPlayers, IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerCheckRoundEnd
	{
		private readonly FindersKeepersGamemode plugin;
		public EventHandlers(FindersKeepersGamemode plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!plugin.Enabled)
				return;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(20, "<color=green>Finder's Keepers</color> gamemode is starting!", false);
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled)
				return;

			plugin.RoundStarted = true;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Scp079Rooms.Clear();
			foreach (Room room in plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
				if (room.ZoneType == ZoneType.HCZ)
					plugin.Scp079Rooms.Add(room);
			foreach (Player player in plugin.Server.GetPlayers())
				Timing.RunCoroutine(plugin.Functions.SpawnClassD(player));

			plugin.Coroutines.Add(Timing.RunCoroutine(plugin.Functions.HczBlackout()));
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.Enabled)
				return;
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.Enabled)
				return;
			plugin.Functions.EndGamemodeRound();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.Enabled)
				return;
			
			if (plugin.Server.Round.Duration < 10)
				return;

			ev.Status = ROUND_END_STATUS.ON_GOING;
			foreach (Player player in plugin.Server.GetPlayers())
				foreach (Smod2.API.Item item in player.GetInventory())
					if (item.ItemType == ItemType.COIN)
					{
						plugin.Server.Map.Broadcast(10, $"Winner, winner chicken dinner! {player.Name} has found the magic coin!", false);
						ev.Status = ROUND_END_STATUS.NO_VICTORY;
						plugin.Functions.EndGamemodeRound();
					}
			
		}
	}
}