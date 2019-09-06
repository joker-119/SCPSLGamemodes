using MEC;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;

namespace FindersKeepersGamemode
{
	public class EventHandlers : IEventHandlerWaitingForPlayers, IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerCheckRoundEnd, IEventHandlerPlayerJoin
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
			Timing.RunCoroutine(plugin.Functions.SpawnCoin());
			
			string[] dList = new string[] { "CHECKPOINT_LCZ_A", "CHECKPOINT_LCZ_B", "CHECKPOINT_ENT", "173" };

			foreach (string d in dList)
			foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
				if (d == door.Name)
				{
					plugin.Info("Locking " + door.Name + ".");
					door.Open = false;
					door.Locked = true;
				}

			string[] oList = new string[] { "HID", "106_BOTTOM", "106_PRIMARY", "106_SECONDARY", "079_SECOND", "079_FIRST", "096", "HCZ_ARMORY", "NUKE_ARMORY", "049_ARMORY" };

			foreach (string o in oList)
			foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
				if (o == door.Name)
				{
					plugin.Info("Opening " + door.Name + ".");
					door.Open = true;
					door.Locked = true;
				}

			plugin.Coroutines.Add(Timing.RunCoroutine(plugin.Functions.HczBlackout()));
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted)
				return;
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted)
				return;
			plugin.Functions.EndGamemodeRound();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted)
				return;

			if (plugin.Server.Round.Duration < 30 || plugin.Functions.IsWinner() == null)
			{
				ev.Status = ROUND_END_STATUS.ON_GOING;
				return;
			}
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(10, $"Winner, winner, chicken dinner! {plugin.Functions.IsWinner().Name} has found the lucky coin!", false);
			ev.Status = ROUND_END_STATUS.MTF_VICTORY;
			
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (plugin.Enabled && !plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(20, "<color=green>Finder's Keepers</color> gamemode is starting!", false);
			}

			if (plugin.RoundStarted)
			{
				ev.Player.PersonalBroadcast(10, "Now playing: <color=green>Finder's Keepers</color> gamemode!", false);
				Timing.RunCoroutine(plugin.Functions.SpawnClassD(ev.Player));
			}
		}
	}
}