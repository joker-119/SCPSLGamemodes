using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace LurkingGamemode
{
	public class Functions
	{
		private readonly Lurking plugin;
		public Functions(Lurking plugin) => this.plugin = plugin;

		
		public bool IsAllowed(ICommandSender sender)
		{
			Player player = sender as Player;

			if (player != null)
			{
				List<string> roleList = (plugin.ValidRanks != null && plugin.ValidRanks.Length > 0) ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

				if (roleList != null && roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
					return true;
				else if (roleList == null || roleList.Count == 0)
					return true;
				else
					return false;
			}
			return true;
		}

		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(25, "<color=#2D2B2B> Lurking in the dark</color> gamemode starting..", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

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