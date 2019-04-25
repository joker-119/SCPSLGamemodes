using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using System.Collections.Generic;
using MEC;

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
			// int r = plugin.Gen.Next(1, plugin.Rooms.Count);
			// Vector spawn = new Vector(plugin.Rooms[r].Position.x, plugin.Rooms[r].Position.y + 2, plugin.Rooms[r].Position.z);

			return plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);
		}

		public IEnumerator<float> SpawnNTF(Player player)
		{
			yield return Timing.WaitForSeconds(1.5f);

			plugin.NTF.Add(player);

			player.ChangeRole(Role.NTF_COMMANDER, false, false, false, false);

			player.Teleport(plugin.NTFSpawn);

			player.SetAmmo(AmmoType.DROPPED_5, plugin.NTFAmmo);

			player.SetHealth(plugin.NTFHealth);

			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			yield return Timing.WaitForSeconds(1);

			foreach (ItemType item in plugin.NTFItems)
			{
				player.GiveItem(item);
				yield return Timing.WaitForOneFrame;
			}
		}

		public IEnumerator<float> SpawnZombie(Player player)
		{
			yield return Timing.WaitForSeconds(1f);

			int r = plugin.Gen.Next(1, plugin.Rooms.Count);
			Vector spawn = new Vector(plugin.Rooms[r].Position.x, plugin.Rooms[r].Position.y + 2, plugin.Rooms[r].Position.z);

			player.ChangeRole(Role.SCP_049_2, false, false, false, false);
			yield return Timing.WaitForSeconds(2);

			player.Teleport(plugin.Server.Map.GetRandomSpawnPoint(Role.FACILITY_GUARD));

			player.SetHealth(plugin.ZHealth);
		}

		public IEnumerator<float> SpawnAmmo(float delay)
		{
			plugin.Info("Ammo Delay: " + delay);
			while (plugin.RoundStarted)
			{
				yield return Timing.WaitForSeconds(delay);

				foreach (Player player in plugin.NTF)
					player.SetAmmo(AmmoType.DROPPED_5, plugin.NTFAmmo);

				plugin.Server.Map.Broadcast(10, "An ammo drop has occured!", false);
			}
		}

		public IEnumerator<float> SpawnCarePackage(float delay)
		{
			plugin.Info("Package Delay: " + delay);

			while (plugin.RoundStarted)
			{
				yield return Timing.WaitForSeconds(delay);

				foreach (Player player in plugin.NTF)
					plugin.Server.Map.SpawnItem(plugin.CarePackage, GetCarePackageDrop(player), Vector.Zero);
			}
		}

		public Vector GetCarePackageDrop(Player player)
		{
			foreach (Room room in plugin.Rooms.Where(r => Vector.Distance(player.GetPosition(), r.Position) <= 30f && Vector.Distance(player.GetPosition(), r.Position) > 10f))
				return room.Position;
			return player.GetPosition();
		}

		public IEnumerator<float> EndRound(float delay)
		{
			plugin.Info("Round Delay: " + delay);
			yield return Timing.WaitForSeconds(delay);

			if (!plugin.RoundStarted) yield break;

			plugin.Server.Map.Broadcast(10, "The NTF have survived the outbreak!", false);
			EndGamemodeRound();
		}

		public IEnumerator<float> LCZDecon(float delay)
		{
			plugin.Info("LCZ Delay: " + delay);
			yield return Timing.WaitForSeconds(delay);

			PlayerManager.localPlayer.GetComponent<DecontaminationLCZ>().time = 666f;
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