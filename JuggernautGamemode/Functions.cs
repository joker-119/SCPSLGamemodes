using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;
using UnityEngine;

namespace JuggernautGamemode
{
	public class Functions
	{
		private readonly Juggernaut plugin;
		public Functions(Juggernaut plugin) => this.plugin = plugin;

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
			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(10, "<color=#228B22>Juggernaut Gamemode</color> is starting..", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}


		public bool IsJuggernaut(Player player)
		{
			if (plugin.Jugg == null) return false;
			
			return player.Name == plugin.Jugg.Name || player.SteamId == plugin.Jugg.SteamId;
		}

		public Player GetJuggernautPlayer()
		{
			foreach (Player player in plugin.Server.GetPlayers())
				if (IsJuggernaut(player))
					return player;
				else
					plugin.Warn("Juggernaut not found!");

			return null;
		}

		private static Vector GetRandomPdExit()
		{
			GameObject[] exitsArray = GameObject.FindGameObjectsWithTag("RoomID");

			List<Vector3> list = (from exit in exitsArray where exit.GetComponent<Rid>() != null select exit.transform.position).ToList();

			Vector3 chosenExit = list[Random.Range(0, list.Count)];
			Vector smodExit = new Vector(chosenExit.x, chosenExit.y += 2f, chosenExit.z);
			
			return smodExit;
		}

		public void CriticalHitJuggernaut(Player player)
		{
			Vector position = GetRandomPdExit();
			int damage = (int)(plugin.JuggHealth * plugin.CriticalDamage);

			player.Damage(damage, DamageType.FRAG);

			player.Teleport(position);

			plugin.Server.Map.Broadcast(10, "The <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
			plugin.Debug("Juggernaut Disarmed & Teleported");
		}

		public void CriticalHitJuggernaut(Player player, Player activator)
		{
			Vector position = GetRandomPdExit();
			int damage = (int)(plugin.JuggHealth * plugin.CriticalDamage);

			player.Damage(damage, DamageType.FRAG);

			player.Teleport(position);

			plugin.Server.Map.Broadcast(10, "" + activator.Name + " has sacrificed themselves and made the <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
			plugin.Debug("Juggernaut Disarmed & Teleported");
		}

		public void ResetJuggernaut(Player player)
		{
			if (plugin.JuggernautPrevRank != null && plugin.JuggernautPrevRank.Length == 2)
				player.SetRank(plugin.JuggernautPrevRank[0], plugin.JuggernautPrevRank[1]);
			else
				plugin.Jugg.SetRank();
			ResetJuggernaut();
		}

		public void ResetJuggernaut()
		{
			plugin.Info("Resetting plugin.");
			plugin.Jugg = null;
			plugin.JuggernautPrevRank = null;
			plugin.SelectedJugg = null;
			plugin.JuggHealth = 0;
		}

		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			ResetJuggernaut();
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}

		public IEnumerator<float> SpawnAsNtfCommander(Player player)
		{
			player.ChangeRole(Role.NTF_COMMANDER, false, true, false);
			yield return Timing.WaitForSeconds(2);

			foreach (Smod2.API.Item item in player.GetInventory()) item.Remove();

			player.SetHealth(plugin.NtfHealth);

			player.SetAmmo(AmmoType.DROPPED_5, plugin.NtfAmmo);
			player.SetAmmo(AmmoType.DROPPED_7, plugin.NtfAmmo);
			player.SetAmmo(AmmoType.DROPPED_9, plugin.NtfAmmo);

			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.RADIO);
			player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			player.GiveItem(ItemType.FLASHBANG);
			player.GiveItem(ItemType.MEDKIT);
			player.GiveItem(ItemType.MTF_COMMANDER_KEYCARD);
			player.GiveItem(ItemType.FRAG_GRENADE);
			if (plugin.NtfDisarmer)
				player.GiveItem(ItemType.DISARMER);

			int ran = plugin.Gen.Next(1, 100);

			if (ran > 75) player.GiveItem(ItemType.MICROHID);

			player.PersonalClearBroadcasts();

			if (plugin.Jugg != null)
				player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut " + plugin.Jugg.Name + "</color>", false);
			else
				player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut</color>", false);
		}

		public void SpawnAsJuggernaut(Player player)
		{

			//Spawned as Juggernaut in 939s spawn location
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);
			player.ChangeRole(Role.CHAOS_INSURGENCY, false, false, true, true);
			player.Teleport(spawn);

			plugin.JuggernautPrevRank = new string[] { player.GetUserGroup().Color, player.GetUserGroup().Name };

			// Given a Juggernaut badge
			player.SetRank("silver", "Juggernaut");

			// Health scales with amount of players in round
			int health = plugin.JuggBase + plugin.JuggIncrease * plugin.Server.NumPlayers - 500;
			player.SetHealth(health);
			plugin.JuggHealth = health;

			// Clear Inventory
			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			//Increased Ammo
			player.SetAmmo(AmmoType.DROPPED_7, 2000);
			player.SetAmmo(AmmoType.DROPPED_5, 2000);
			player.SetAmmo(AmmoType.DROPPED_9, 2000);

			// 1 Logicer
			player.GiveItem(ItemType.LOGICER);

			// 1 O5 Keycard
			player.GiveItem(ItemType.O5_LEVEL_KEYCARD);

			// Frag Grenades
			for (int i = 0; i < plugin.JuggGrenades; i++) player.GiveItem(ItemType.FRAG_GRENADE);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are the <color=#228B22>Juggernaut</color> Eliminate all <color=#002DB3>NTF Commanders</color>", false);
		}

		public static string DrawHealthBar(double percentage)
		{
			string bar = "<color=#FFFFFF>(</color>";
			const int barSize = 20;

			percentage *= 100;
			if (percentage == 0) return "(      )";

			for (double i = 0; i < 100; i += 100 / barSize)
				if (i < percentage)
					bar += "█";
				else
					bar += "<color=#474747>█</color>";

			bar += "<color=#ffffff>)</color>";

			return bar;
		}
	}
}