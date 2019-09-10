using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace ZombielandGamemode
{
	public class Methods
	{
		private readonly Zombie plugin;
		public Methods(Zombie plugin) => this.plugin = plugin;

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
			plugin.Server.Map.Broadcast(25, "<color=#07A407>Zombieland</color> gamemode is starting..", false);
		}
		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public Vector Spawn() => plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);

		public IEnumerator<float> SpawnNtf(Player player)
		{
			yield return Timing.WaitForSeconds(1.5f);

			plugin.Ntf.Add(player);

			player.ChangeRole(Role.NTF_COMMANDER, false, false, false);

			player.Teleport(plugin.NtfSpawn);

			player.SetAmmo(AmmoType.DROPPED_5, plugin.NtfAmmo);

			player.SetHealth(plugin.NtfHealth);

			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			yield return Timing.WaitForSeconds(1);

			player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.FRAG_GRENADE);
		}

		public IEnumerator<float> SpawnZombie(Player player)
		{
			yield return Timing.WaitForSeconds(1f);

			player.ChangeRole(Role.SCP_049_2, false, false, false);
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

				foreach (Player player in plugin.Ntf)
					player.SetAmmo(AmmoType.DROPPED_5, plugin.NtfAmmo);

				plugin.Server.Map.Broadcast(10, "An ammo drop has occured!", false);
			}
		}

		public IEnumerator<float> SpawnCarePackage(float delay)
		{
			plugin.Info("Package Delay: " + delay);

			while (plugin.RoundStarted)
			{
				yield return Timing.WaitForSeconds(delay);

				foreach (Player player in plugin.Ntf)
					plugin.Server.Map.SpawnItem(plugin.CarePackage, GetCarePackageDrop(player), Vector.Zero);
			}
		}

		private Vector GetCarePackageDrop(Player player)
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

		public IEnumerator<float> LczDecon(float delay)
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