using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;
using UnityEngine;

namespace RisingLavaGamemode
{
	public class Methods
	{
		private readonly RisingLavaGamemode plugin;
		public Methods(RisingLavaGamemode plugin) => this.plugin = plugin;
		
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
				plugin.Server.Map.Broadcast(25, "<color=#123456>Rising Lava Gamemode is starting..</color>", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}


		public void Get079Rooms()
		{
			foreach (Room room in plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
				plugin.Rooms.Add(room);
		}

		public IEnumerator<float> LczLockdown(float delay)
		{
			yield return Timing.WaitForSeconds(delay);

			foreach (Player player in plugin.Server.GetPlayers())
				if (plugin.Rooms.Where(rm => rm.ZoneType == ZoneType.LCZ)
					.Any(room => player.GetPosition().y == room.Position.y))
					Timing.RunCoroutine(KillPlayer(player));

			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors()
				.Where(dr => dr.Name == "CHECKPOINT_LCZ_A" || dr.Name == "CHECKPOINT_LCZ_B"))
			{
				door.Locked = true;
				door.Open = false;
			}

			Timing.RunCoroutine(HczLockdown(plugin.HczDelay));
		}

		public IEnumerator<float> HczLockdown(float delay)
		{
			yield return Timing.WaitForSeconds(delay);

			foreach (Player player in plugin.Server.GetPlayers())
				if (plugin.Rooms.Where(rm => rm.ZoneType == ZoneType.HCZ)
					.Any(room => player.GetPosition().y == room.Position.y))
					Timing.RunCoroutine(KillPlayer(player));

			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors().Where(dr => dr.Name == "CHECKPOINT_ENT"))
			{
				door.Locked = true;
				door.Open = false;
			}

			Timing.RunCoroutine(EntLockdown(plugin.EntDelay));
		}

		public IEnumerator<float> EntLockdown(float delay)
		{
			yield return Timing.WaitForSeconds(delay);

			foreach (Player player in plugin.Server.GetPlayers())
				if (plugin.Rooms.Where(rm => rm.ZoneType == ZoneType.ENTRANCE)
					.Any(room => player.GetPosition().y == room.Position.y))
					Timing.RunCoroutine(KillPlayer(player));
			
			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors().Where(dr => dr.Name == "GATE_A" || dr.Name == "GATE_B"))
			{
				door.Locked = true;
				door.Open = false;
			}

			foreach (GameObject ply in PlayerManager.singleton.players)
				ply.GetComponent<WeaponManager>().NetworkfriendlyFire = true;
		}

		private static IEnumerator<float> KillPlayer(Player player)
		{
			while (true)
			{
				if (player.TeamRole.Role == Role.SPECTATOR) break;
				
				player.ThrowGrenade(GrenadeType.FRAG_GRENADE, false, Vector.Zero, true, player.GetPosition(), false, 0f);

				yield return Timing.WaitForSeconds(0.5f);
			}
		}
	}
}