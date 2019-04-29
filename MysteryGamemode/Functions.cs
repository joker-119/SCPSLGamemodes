using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace Mystery
{
	public class Functions
	{
		private readonly Mystery plugin;

		public Functions(Mystery plugin) => this.plugin = plugin;

		public bool IsAllowed(ICommandSender sender)
		{
			if (!(sender is Player player)) return true;
			
			List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0 ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

			if (roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
				return true;
			
			return roleList.Count == 0;
		}

		public void EndGamemodeRound()
		{
			plugin.Info("Endgame function.");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			plugin.Murd.Clear();
		}

		public IEnumerator<float> SpawnMurd(Player player)
		{
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.CLASSD);

			player.ChangeRole(Role.CLASSD, false, false, false);

			yield return Timing.WaitForSeconds(1);

			player.Teleport(spawn);

			foreach (Smod2.API.Item item in player.GetInventory()) item.Remove();

			player.GiveItem(ItemType.USP);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);
			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.COIN);
			player.GiveItem(ItemType.RADIO);

			player.SetAmmo(AmmoType.DROPPED_5, 500);
			player.SetAmmo(AmmoType.DROPPED_7, 500);
			player.SetAmmo(AmmoType.DROPPED_9, 500);

			player.SetHealth(plugin.MurdHealth);

			plugin.Murd.Add(player.SteamId, true);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are a <color=#c50000> Murderer</color>. You must murder all of the Civilians before the detectives find and kill you.", false);
		}
		
		public IEnumerator<float> SpawnDet(Player player)
		{
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCIENTIST);

			player.ChangeRole(Role.SCIENTIST, false, false, false);

			yield return Timing.WaitForSeconds(1);

			player.Teleport(spawn);

			foreach (Smod2.API.Item item in player.GetInventory()) item.Remove();

			player.GiveItem(ItemType.COM15);
			player.GiveItem(ItemType.CONTAINMENT_ENGINEER_KEYCARD);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.DISARMER);

			player.SetHealth(plugin.DetHealth);

			player.SetAmmo(AmmoType.DROPPED_9, 500);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are a <color=#DAD530> Detective</color>. You must find all of the Murderers before they kill all of the Civilians!", false);
		}
		
		public IEnumerator<float> SpawnCiv(Player player)
		{
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.CLASSD);

			player.ChangeRole(Role.CLASSD, false, false, false);

			yield return Timing.WaitForSeconds(1);

			player.Teleport(spawn);

			foreach (Smod2.API.Item item in player.GetInventory()) item.Remove();

			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.JANITOR_KEYCARD);
			player.GiveItem(ItemType.COIN);
			player.GiveItem(ItemType.CUP);

			player.SetHealth(plugin.CivHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are a <color=#5AD3D9>Civilian</color>. You must help the Detectives find the murderers, before they kill all of your friends!", false);
		}
	}
}