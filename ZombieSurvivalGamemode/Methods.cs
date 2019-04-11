using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using System.Collections.Generic;
using scp4aiur;

namespace ZombieSurvival
{
	public class Methods
	{
		private readonly Zombie plugin;
		public Methods(Zombie plugin) => this.plugin = plugin;

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
				plugin.Server.Map.Broadcast(25, "<color=#07A407>Zombie Survival</color> gamemode is starting..", false);
			}
		}
		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public Vector Spawn()
		{
			int r = plugin.Gen.Next(1, plugin.Rooms.Count);
			Vector spawn = new Vector(plugin.Rooms[r].Position.x, plugin.Rooms[r].Position.y + 2, plugin.Rooms[r].Position.z);

			return spawn;
		}

		public IEnumerable<float> SpawnNTF(Player player)
		{
			yield return 0.1f;

			plugin.NTF.Add(player);

			player.ChangeRole(Role.NTF_COMMANDER, false, false, false, false);

			player.Teleport(plugin.NTFSpawn);

			player.SetAmmo(AmmoType.DROPPED_5, plugin.NTFAmmo);

			player.SetHealth(plugin.NTFHealth);

			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			foreach (ItemType item in plugin.NTFItems)
				player.GiveItem(item);
		}

		public IEnumerable<float> SpawnZombie(Player player)
		{
			yield return 0.1f;

			int r = plugin.Gen.Next(1, plugin.Rooms.Count);
			Vector spawn = new Vector(plugin.Rooms[r].Position.x, plugin.Rooms[r].Position.y + 2, plugin.Rooms[r].Position.z);

			player.ChangeRole(Role.SCP_049_2, false, false, false, false);

			player.Teleport(spawn);

			player.SetHealth(plugin.ZHealth);
		}

		public IEnumerable<float> SpawnAmmo()
		{
			yield return plugin.AmmoTimer;

			foreach (Player player in plugin.NTF)
				player.SetAmmo(AmmoType.DROPPED_5, plugin.NTFAmmo);

			plugin.Server.Map.Broadcast(10, "An ammo drop has occured!", false);
		}

		public IEnumerable<float> SpawnCarePackage()
		{
			yield return plugin.CarePackageTimer;

			foreach (Player player in plugin.NTF)
				plugin.Server.Map.SpawnItem(plugin.CarePackage, GetCarePackageDrop(player), Vector.Zero);
		}

		public Vector GetCarePackageDrop(Player player)
		{
			foreach (Room room in plugin.Rooms.Where(r => Vector.Distance(player.GetPosition(), r.Position) <= 30f && Vector.Distance(player.GetPosition(), r.Position) > 10f))
				return room.Position;
			return player.GetPosition();
		}

		public IEnumerable<float> EndRound()
		{
			yield return plugin.RoundTimer;

			plugin.Server.Map.Broadcast(10, "The NTF have survived the outbreak!", false);
			EndGamemodeRound();
		}

		public void EndGamemodeRound()
		{
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}

		public void Get079Rooms()
		{
			foreach (Room room in plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(r => r.ZoneType != ZoneType.LCZ && r.ZoneType != ZoneType.UNDEFINED))
				plugin.Rooms.Add(room);
		}
	}
}