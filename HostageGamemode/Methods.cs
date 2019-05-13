using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;

namespace HostageGamemode
{
	public class Methods
	{
		private readonly HostageGamemode plugin;
		public Methods(HostageGamemode plugin) => this.plugin = plugin;

		public IEnumerator<float> SpawnCriminal(Player player)
		{
			player.ChangeRole(Role.CLASSD, false, false, false);
			yield return Timing.WaitForSeconds(1f);
			
			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			player.GiveItem(ItemType.COM15);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.FACILITY_MANAGER_KEYCARD);
			player.GiveItem(ItemType.FRAG_GRENADE);
			player.GiveItem(ItemType.FLASHBANG);
			player.GiveItem(ItemType.DISARMER);
			
			player.Teleport(plugin.CriminalSpawn);
			player.SetHealth(plugin.CriminalHealth);
			
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(10, "You are a Criminal. Secure the hostage and prevent NTF from freeing them.", false);
		}

		public IEnumerator<float> SpawnPolice(Player player)
		{
			player.ChangeRole(Role.FACILITY_GUARD, false, false, false);
			yield return Timing.WaitForSeconds(1f);
			
			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			player.GiveItem(ItemType.FACILITY_MANAGER_KEYCARD);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.RADIO);
			player.GiveItem(ItemType.FLASHBANG);
			player.GiveItem(ItemType.P90);
			
			player.Teleport(plugin.PoliceSpawn);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(10, "You are a Police Officer. Find and secure the hostage, eliminate criminals.", false);
		}

		public IEnumerator<float> SpawnHostage(Player player)
		{
			player.ChangeRole(Role.SCIENTIST, false, false, false);
			yield return Timing.WaitForSeconds(1f);
			
			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			player.GiveItem(ItemType.CUP);
			
			player.Teleport(plugin.Server.Map.GetRandomSpawnPoint(Role.FACILITY_GUARD));
			player.SetHealth(plugin.HostageHealth);
			
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(10, "You are a hostage. Evade the criminals, find the police to be escorted out. Co-operate with criminals if captured.", false);
		}

		public IEnumerator<float> SpawnSwat(Player player)
		{
			player.ChangeRole(Role.NTF_LIEUTENANT);
			yield return Timing.WaitForSeconds(1f);
			
			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			player.GiveItem(ItemType.FACILITY_MANAGER_KEYCARD);
			player.GiveItem(ItemType.RADIO);
			player.GiveItem(ItemType.FRAG_GRENADE);
			player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			player.GiveItem(ItemType.MEDKIT);
			
			player.Teleport(plugin.Server.Map.GetRandomSpawnPoint(Role.NTF_CADET));
			player.SetHealth(plugin.SwatHealth);
			
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(10, "You are a swat officer. Infiltrate the facility and secure the hostage!", false);
		}

		public IEnumerator<float> SwatArrival()
		{
			List<string> doorNames = new List<string> { "GATE_A", "GATE_B", "CHECKPOINT_ENT" };
			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors()
				.Where(dr => doorNames.Contains(dr.Name)))
			{
				door.Open = false;
				door.Locked = true;
			}

			yield return Timing.WaitForSeconds(plugin.SwatDelay);

			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors()
				.Where(dr => dr.Name == "GATE_A" || dr.Name == "GATE_B"))
				door.Locked = false;
		}
		
		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}
	}
}