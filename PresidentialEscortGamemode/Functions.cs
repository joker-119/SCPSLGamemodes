using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace PresidentialEscortGamemode
{
	public class Functions
	{
		private readonly PresidentialEscort plugin;

		public Functions(PresidentialEscort plugin) => this.plugin = plugin;

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
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			plugin.Vip = null;
			plugin.VipEscaped = false;
		}

		public IEnumerator<float> AnnounceLocation(float delay)
		{
			yield return Timing.WaitForSeconds(delay);
			while (plugin.RoundStarted)
			{
				Vector loc = plugin.Vip.GetPosition();

				foreach (Room room in plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(r => Vector.Distance(loc, r.Position) <= 10f))
				{
					ZoneType zone = room.ZoneType;

					switch (zone)
					{
						case ZoneType.HCZ:
							plugin.Server.Map.Broadcast(10, "The VIP is in Heavy Containment!", false);
							break;
						case ZoneType.LCZ:
							plugin.Server.Map.Broadcast(10, "The VIP is in Light Containment!", false);
							break;
						case ZoneType.ENTRANCE:
							plugin.Server.Map.Broadcast(10, "The VIP is in Entrance Zone!", false);
							break;
						case ZoneType.UNDEFINED:
							plugin.Debug("VIP location undefined.");
							break;
						default:
							plugin.Server.Map.Broadcast(10, "The VIP is on the Surface!", false);
							break;
					}
					break;
				}
				yield return Timing.WaitForSeconds(120f);
			}
		}

		public IEnumerator<float> SpawnVip(Player player)
		{
			plugin.Vip = player;
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.CLASSD);

			player.ChangeRole(Role.SCIENTIST, false, false);

			yield return Timing.WaitForSeconds(2);
			player.Teleport(spawn);

			foreach (Smod2.API.Item item in player.GetInventory()) item.Remove();

			player.GiveItem(ItemType.MAJOR_SCIENTIST_KEYCARD);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.RADIO);
			player.GiveItem(ItemType.FLASHLIGHT);

			player.SetHealth(plugin.VipHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are the <color=#f8ea56>VIP</color> Escape the facility with the help of " +
				"<color=#308ADA>NTF</color> while avoiding the <color=#e83e25>SCPs</color>.", false);

		}

		public IEnumerator<float> SpawnNtf(Player player)
		{
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
			player.ChangeRole(Role.FACILITY_GUARD, false, true, false);

			yield return Timing.WaitForSeconds(2);
			player.Teleport(spawn);

			foreach (Smod2.API.Item item in player.GetInventory()) item.Remove();

			player.SetAmmo(AmmoType.DROPPED_5, 500);
			player.SetAmmo(AmmoType.DROPPED_7, 500);
			player.SetAmmo(AmmoType.DROPPED_9, 500);

			player.GiveItem(ItemType.RADIO);
			player.GiveItem(ItemType.P90);
			player.GiveItem(ItemType.FRAG_GRENADE);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.SENIOR_GUARD_KEYCARD);
			player.GiveItem(ItemType.FLASHLIGHT);
			
			player.SetHealth(plugin.GuardHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are an <color=#308ADA>NTF Cadet</color>. Work with others to help the " +
				"<color=#f8ea56>VIP</color> escape and eliminate the <color=#e83e25>SCPs</color>", false);
		}
	}
}