using System;
using System.Collections.Generic;
using MEC;
using Smod2.API;

namespace MassacreGamemode
{
	public class Functions
	{
		private readonly Massacre plugin;

		public Functions(Massacre plugin) => this.plugin = plugin;

		public Vector SpawnLoc()
		{
			Vector spawn;

			switch (plugin.SpawnRoom.ToLower())
			{
				case "jail":
					{
						plugin.Info("Jail room selected.");
						spawn = new Vector(53, 1020, -44);

						return spawn;
					}
				case "939":
					{
						plugin.Info("939 Spawn Room selected");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);

						return spawn;
					}
				case "049":
					{
						plugin.Info("049 Spawn room selected");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);

						return spawn;
					}
				case "106":
					{
						plugin.Info("106 Spawn room selected");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_106);

						return spawn;
					}
				case "173":
					{
						plugin.Info("173 Spawn room selected.");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_173);

						return spawn;
					}
				case "random":
					{
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53));
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_173));
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049));
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_106));
						plugin.SpawnLocs.Add(new Vector(53, 1020, -44));

						int randomInt = new Random().Next(plugin.SpawnLocs.Count);

						return plugin.SpawnLocs[randomInt];
					}
				default:
					{
						plugin.Info("Invalid location selected, defaulting to Jail.");
						spawn = new Vector(53, 1020, -44);

						return spawn;
					}
			}
		}
		public IEnumerator<float> SpawnDboi(Player player)
		{
			player.ChangeRole(Role.CLASSD, false, false, false, true);

			player.Teleport(plugin.SpawnLoc);

			yield return Timing.WaitForSeconds(2);

			foreach (Item item in player.GetInventory()) item.Remove();

			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.CUP);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Get ready to die!", false);
		}
		
		public IEnumerator<float> SpawnNut(Player player)
		{
			player.ChangeRole(Role.SCP_173, false, false, false);

			yield return Timing.WaitForSeconds(5.5f);

			player.SetGodmode(false);

			player.Teleport(plugin.SpawnLoc);

			plugin.Info("Spawned " + player.Name + " as SCP-173");
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(35, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the D-bois!", false);

			player.SetHealth(plugin.NutHealth > 0? plugin.NutHealth: 1);
		}
		
		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}
	}
}