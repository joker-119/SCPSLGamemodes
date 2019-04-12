using System.Linq;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System;
using Smod2.Commands;

namespace Gungame
{
	public class Functions
	{
		private readonly GunGame plugin;
		public Functions(GunGame plugin) => this.plugin = plugin;

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
				plugin.Server.Map.Broadcast(10, "<color=#5D9AAC>GunGame Gamemode</color> is starting..", false);
			}
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
			player.ChangeRole(Role.CLASSD, false, false, false, false);
			player.Teleport(new Vector(GetSpawn().x, (GetSpawn().y + 3), GetSpawn().z));
			yield return 1;

			player.SetGodmode(false);
			player.SetHealth(plugin.Health);

			foreach (Smod2.API.Item item in player.GetInventory())
			{
				item.Remove();
			}

			if (plugin.Reversed)
				player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			else
				player.GiveItem(ItemType.FRAG_GRENADE);

			player.GiveItem(ItemType.MEDKIT);
		}

		public void LockDoors()
		{
			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors())
			{
				if (door.Name.Contains("CHECKPOINT") || door.Name.Contains("079") || door.Name.Contains("106"))
					door.Locked = true;
				else
					door.Locked = false;
			}
		}

		public void ReplaceGun(Player player)
		{
			if (plugin.Reversed)
			{
				foreach (Smod2.API.Item item in player.GetInventory())
				{
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
				}
			}
			else
				foreach (Smod2.API.Item item in player.GetInventory())
				{
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

			player.SetAmmo(AmmoType.DROPPED_5, 500);
			player.SetAmmo(AmmoType.DROPPED_7, 500);
			player.SetAmmo(AmmoType.DROPPED_9, 500);
		}

		public void AnnounceWinner(Player player)
		{
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(15, "We have out champion! Congratulations " + player.Name + "!", false);
			EndGamemodeRound();
		}

		public Room GetRooms(ZoneType zone)
		{
			List<Room> rooms = new List<Room>();

			List<RoomType> validRooms = new List<RoomType>() { RoomType.SCP_096, RoomType.SERVER_ROOM, RoomType.ENTRANCE_CHECKPOINT, RoomType.HCZ_ARMORY, RoomType.MICROHID };

			foreach (Room room in plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(rm => rm.ZoneType == zone))
			{
				if (validRooms.Contains(room.RoomType))
					rooms.Add(room);
			}

			int r = plugin.Gen.Next(rooms.Count);
			return rooms[r];
		}

		public Vector GetSpawn()
		{
			switch (plugin.Zone.ToLower())
			{
				case "lcz":
					return plugin.Server.Map.GetRandomSpawnPoint(Role.SCIENTIST);
				case "hzc":
					return GetRooms(ZoneType.HCZ).Position;
				case "enterance":
				case "ent":
					return plugin.Server.Map.GetRandomSpawnPoint(Role.FACILITY_GUARD);
				default:
					return new Vector(53, 1020, -44);
			}
		}
	}
}