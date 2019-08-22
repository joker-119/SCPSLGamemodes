using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace Gungame
{
	public class Functions
	{
		private readonly GunGame plugin;
		public Functions(GunGame plugin) => this.plugin = plugin;

		public bool IsAllowed(ICommandSender sender)
		{
			if (!(sender is Player player)) return true;
			
			List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0 ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

			if (roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
				return true;
			return roleList.Count == 0;
		}
		
		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (plugin.RoundStarted) return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(10, "<color=#5D9AAC>GunGame Gamemode</color> is starting..", false);
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.Info("EndGamemode Round");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			plugin.Winner = null;
		}

		public IEnumerator<float> Spawn(Player player)
		{
			player.PersonalBroadcast(5, "You will respawn shortly..", false);
			yield return Timing.WaitForSeconds(3);

			plugin.Info("Spawning player: " + player.Name);
			player.ChangeRole(Role.CLASSD, false, false, false);
			plugin.Info("Teleporting player: " + player.Name);
			player.Teleport(GetSpawn());
			yield return Timing.WaitForSeconds(1);

			player.SetGodmode(false);
			plugin.Info("Setting health for player: " + player.Name);
			player.SetHealth(plugin.Health);

			foreach (Smod2.API.Item item in player.GetInventory()) item.Remove();
			plugin.Info("Setting inventory for player: " + player.Name);

			player.GiveItem(plugin.Reversed ? ItemType.E11_STANDARD_RIFLE : ItemType.FRAG_GRENADE);

			player.GiveItem(ItemType.MEDKIT);

			player.SetAmmo(AmmoType.DROPPED_5, 500);
			player.SetAmmo(AmmoType.DROPPED_7, 500);
			player.SetAmmo(AmmoType.DROPPED_9, 500);
		}

		public void ReplaceGun(Player player)
		{
			if (plugin.Reversed)
				foreach (Smod2.API.Item item in player.GetInventory())
					switch (item.ItemType)
					{
						case ItemType.E11_STANDARD_RIFLE:
							item.Remove();
							player.GiveItem(ItemType.P90);
							break;
						case ItemType.P90:
							item.Remove();
							player.GiveItem(ItemType.LOGICER);
							break;
						case ItemType.LOGICER:
							item.Remove();
							player.GiveItem(ItemType.MP4);
							break;
						case ItemType.MP4:
							item.Remove();
							player.GiveItem(ItemType.USP);
							break;
						case ItemType.USP:
							item.Remove();
							player.GiveItem(ItemType.COM15);
							break;
						case ItemType.COM15:
							item.Remove();
							player.GiveItem(ItemType.FRAG_GRENADE);
							break;
					}
			else
				foreach (Smod2.API.Item item in player.GetInventory())
					switch (item.ItemType)
					{
						case ItemType.FRAG_GRENADE:
							item.Remove();
							player.GiveItem(ItemType.COM15);
							break;
						case ItemType.COM15:
							item.Remove();
							player.GiveItem(ItemType.USP);
							break;
						case ItemType.USP:
							item.Remove();
							player.GiveItem(ItemType.MP4);
							break;
						case ItemType.MP4:
							item.Remove();
							player.GiveItem(ItemType.LOGICER);
							break;
						case ItemType.LOGICER:
							item.Remove();
							player.GiveItem(ItemType.P90);
							break;
						case ItemType.P90:
							item.Remove();
							player.GiveItem(ItemType.E11_STANDARD_RIFLE);
							break;
					}
		}

		public void AnnounceWinner(Player player)
		{
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(15, "We have our champion! Congratulations " + player.Name + "!", false);
			EndGamemodeRound();
		}

		private Room GetRooms(ZoneType zone)
		{
			List<RoomType> validRooms = new List<RoomType> { RoomType.SCP_096, RoomType.SERVER_ROOM, RoomType.ENTRANCE_CHECKPOINT, RoomType.HCZ_ARMORY, RoomType.MICROHID };

			List<Room> rooms = plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(rm => rm.ZoneType == zone).Where(room => validRooms.Contains(room.RoomType)).ToList();

			int r = plugin.Gen.Next(rooms.Count);
			return rooms[r];
		}

		private Vector GetSpawn()
		{
			switch (plugin.Zone.ToLower())
			{
				case "lcz":
					return plugin.Server.Map.GetRandomSpawnPoint(Role.SCIENTIST);
				case "hzc":
					return GetRooms(ZoneType.HCZ).Position;
				case "entrance":
				case "ent":
					return plugin.Server.Map.GetRandomSpawnPoint(Role.FACILITY_GUARD);
				default:
					return new Vector(53, 1020, -44);
			}
		}
	}
}