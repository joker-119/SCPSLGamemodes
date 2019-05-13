using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using UnityEngine;

namespace RisingLavaGamemode
{
	public class Methods
	{
		private readonly RisingLavaGamemode plugin;
		public Methods(RisingLavaGamemode plugin) => this.plugin = plugin;

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
				
				player.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, true, player.GetPosition(), false, 0f);

				yield return Timing.WaitForSeconds(0.5f);
			}
		}
	}
}