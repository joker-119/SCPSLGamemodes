using System.Diagnostics.Contracts;
using System.Security;
using Smod2;
using Smod2.API;
using System;
using scp4aiur;
using System.Collections.Generic;

namespace Mystery
{
	public class Functions
	{
		public static Functions singleton;
		public Mystery Mystery;
		public Functions(Mystery plugin)
		{
			this.Mystery = plugin;
			Functions.singleton = this;
		}

		public void DisableGamemode()
		{
			Mystery.enabled = false;
			Mystery.plugin.pluginManager.Server.Map.ClearBroadcasts();
		}
		public void EnableGamemode()
		{
			Mystery.enabled = true;
			if (!Mystery.roundstarted)
			{
				Mystery.plugin.Server.Map.ClearBroadcasts();
				Mystery.plugin.Server.Map.Broadcast(25, "<color=#c50000>Murder Mystery</color> gamemode is starting..", false);	
			}
		}
		public void EndGamemoderound()
		{
			if (Mystery.enabled)
			{
				Mystery.plugin.Info("Endgame function.");
				Mystery.roundstarted = false;
				Mystery.plugin.Server.Round.EndRound();
			}
		}
		public IEnumerable<float> SpawnMurd(Player player)
		{
			Vector spawn = Mystery.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
			player.ChangeRole(Role.CLASSD, false, false, false, false);
			yield return 1;
			player.Teleport(spawn);
			//Delete old inventory and replace with new one
			foreach (Item item in player.GetInventory())
			{
				item.Remove();
			}
			player.GiveItem(ItemType.USP);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);
			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.COIN);
			//Set 500 ammo for all weapons
			player.SetAmmo(AmmoType.DROPPED_5,500);
			player.SetAmmo(AmmoType.DROPPED_7,500);
			player.SetAmmo(AmmoType.DROPPED_9,500);
			//Set player health to config
			player.SetHealth(Mystery.murder_health);
		}
		public IEnumerable<float> SpawnDet(Player player)
		{
			Vector spawn = Mystery.Server.Map.GetRandomSpawnPoint(Role.SCIENTIST);
			player.ChangeRole(Role.SCIENTIST, false, false, false, false);
			yield return 1;
			player.Teleport(spawn);
			//Remove old inventory and replace with new one
			foreach (Item item in player.GetInventory())
			{
				item.Remove();
			}
			player.GiveItem(ItemType.COM15);
			player.GiveItem(ItemType.CONTAINMENT_ENGINEER_KEYCARD);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.FLASHLIGHT);
			//Set player health to config
			player.SetHealth(Mystery.det_health);
			//Set 9mm Ammo to 500
			player.SetAmmo(AmmoType.DROPPED_5,500);
		}
		public IEnumerable<float> SpawnCiv(Player player)
		{
			Vector spawn = Mystery.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
			player.ChangeRole(Role.CLASSD, false, false, false, false);
			yield return 1;
			player.Teleport(spawn);
			foreach (Item item in player.GetInventory())
			{
				item.Remove();
			}
			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.JANITOR_KEYCARD);
			player.GiveItem(ItemType.COIN);
			player.GiveItem(ItemType.CUP);
		}
	}
}