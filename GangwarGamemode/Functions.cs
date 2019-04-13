using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;
using System;
using Smod2.Commands;
using MEC;


namespace Gangwar
{
	public class Functions
	{
		private readonly Gangwar plugin;
		public Functions(Gangwar plugin) => this.plugin = plugin;

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
				plugin.Server.Map.Broadcast(25, "<color=#00ffff> Gangwar Gamemode is starting..</color>", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function.");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			plugin.Spawning.Clear();
		}

		public IEnumerator<float> SpawnChaos(Player player, float delay)
		{
			plugin.Spawning.Add(player.SteamId, true);
			yield return Timing.WaitForSeconds(delay);

			player.ChangeRole(Role.CHAOS_INSURGENCY, false, true, false, true);
			yield return Timing.WaitForSeconds(2);

			foreach (Smod2.API.Item item in player.GetInventory())
			{
				item.Remove();
			}

			player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			player.GiveItem(ItemType.COM15);
			player.GiveItem(ItemType.FRAG_GRENADE);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.FLASHBANG);
			player.GiveItem(ItemType.WEAPON_MANAGER_TABLET);

			player.SetAmmo(AmmoType.DROPPED_5, 500);
			player.SetAmmo(AmmoType.DROPPED_7, 500);
			player.SetAmmo(AmmoType.DROPPED_9, 500);

			player.SetHealth(plugin.CIHealth);
		}

		public IEnumerator<float> SpawnNTF(Player player, float delay)
		{
			plugin.Spawning.Add(player.SteamId, true);
			yield return Timing.WaitForSeconds(delay);

			player.ChangeRole(Role.NTF_COMMANDER, false, true, false, false);
			yield return Timing.WaitForSeconds(2);

			foreach (Smod2.API.Item item in player.GetInventory())
			{
				item.Remove();
			}

			player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			player.GiveItem(ItemType.COM15);
			player.GiveItem(ItemType.FRAG_GRENADE);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.FLASHBANG);
			player.GiveItem(ItemType.WEAPON_MANAGER_TABLET);

			player.SetAmmo(AmmoType.DROPPED_5, 500);
			player.SetAmmo(AmmoType.DROPPED_7, 500);
			player.SetAmmo(AmmoType.DROPPED_9, 500);

			player.SetHealth(plugin.NTFHealth);
		}
	}
}