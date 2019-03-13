using System.Linq;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System;

namespace Gungame
{
	public class Functions
	{
		public static Functions singleton;
		public GunGame GunGame;
		public Functions(GunGame plugin)
		{
			this.GunGame = plugin;
			Functions.singleton = this;
		}

		public void EnableGamemode()
		{
			GunGame.enabled = true;
			if (!GunGame.roundstarted)
			{
				GunGame.Server.Map.ClearBroadcasts();
				GunGame.Server.Map.Broadcast(10, "<color=#5D9AAC>GunGame Gamemode</color> is starting..", false);
			}
		}
		public void DisableGamemode()
		{
			GunGame.enabled = false;
			GunGame.Server.Map.ClearBroadcasts();
		}
		public void EndGamemodeRound()
		{
			GunGame.Info("EndGamemode Round");
			GunGame.roundstarted = false;
			GunGame.Server.Round.EndRound();
		}
		public IEnumerable<float> Spawn(Player player)
		{
			player.ChangeRole(Role.CLASSD, false, false, false, false);
			yield return 2;
			player.Teleport(GetSpawn());
			player.SetHealth(GunGame.health);
			foreach (Item item in player.GetInventory())
			{
				item.Remove();
			}
			if (GunGame.reversed)
				player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			else
				player.GiveItem(ItemType.DISARMER);
			player.GiveItem(ItemType.MEDKIT);
		}
		public void LockDoors()
		{
			foreach (Door door in GunGame.Server.Map.GetDoors())
			{
				if (door.Name.Contains("ZONE"))
					door.Locked = true;
				else
					door.Locked = false;
			}
		}
		public void AnnounceWinner(Player player)
		{
			GunGame.Server.Map.ClearBroadcasts();
			GunGame.Server.Map.Broadcast(15, "We have out champion! Congratulations " + player.Name + "!", false);
			EndGamemodeRound();
		}
		public Room GetRooms(ZoneType zone)
		{
			int ran = GunGame.gen.Next(GunGame.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(p => p.ZoneType == zone).ToList().Count);
			return GunGame.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(p => p.ZoneType == zone).ToList()[ran];
		}
		public Vector GetSpawn()
		{
			switch (GunGame.zone.ToLower())
			{
				case "lcz":
					return GetRooms(ZoneType.LCZ).Position;
				case "hzc":
					return GetRooms(ZoneType.HCZ).Position;
				case "enterance":
				case "ent":
					return GetRooms(ZoneType.ENTRANCE).Position;
				default:
					return new Vector (53, 1020, -44);
			}
		}
	}
}