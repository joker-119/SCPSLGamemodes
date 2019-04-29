using System.Collections.Generic;
using MEC;
using Smod2;
using Smod2.API;

namespace LurkingGamemode
{
	public class Functions
	{
		private readonly Lurking plugin;
		public Functions(Lurking plugin) => this.plugin = plugin;

		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}

		public void Get079Rooms()
		{
			foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
				if (room.ZoneType == ZoneType.LCZ)
					plugin.BlackoutRooms.Add(room);
		}

		public IEnumerator<float> HczBlackout()
		{
			while (plugin.RoundStarted)
			{
				Generator079.generators[0].CallRpcOvercharge();
				yield return Timing.WaitForSeconds(11f);
			}
		}

		public IEnumerator<float> LczBlackout()
		{
			while (plugin.RoundStarted)
			{
				foreach (Room room in plugin.BlackoutRooms)
					room.FlickerLights();

				yield return Timing.WaitForSeconds(8f);
			}
		}

		public void SpawnLarry(Player player)
		{
			player.ChangeRole(Role.SCP_106, false, true, false);

			player.SetHealth(plugin.LarryHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
		}

		public void SpawnDoggo(Player player)
		{
			if (player.TeamRole.Role == Role.SCP_106) return;
			
			player.ChangeRole(Role.SCP_939_53, false, true, false);

			player.SetHealth(plugin.DoggoHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
		}
	}
}