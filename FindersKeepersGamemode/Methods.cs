using System.Collections.Generic;
using System.Linq;
using MEC;
using ServerMod2.API;
using Smod2.API;
using Smod2.Commands;
using UnityEngine;

namespace FindersKeepersGamemode
{
	public class Methods
	{
		private readonly FindersKeepersGamemode plugin;
		public Methods(FindersKeepersGamemode plugin) => this.plugin = plugin;
		
		public bool IsAllowed(ICommandSender sender)
		{
			if (!(sender is Player player)) 
				return true;
			
			List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0 ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

			if (roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
				return true;
			return roleList.Count == 0;
		}

		public IEnumerator<float> HczBlackout()
		{
			for (;;)
			{
				foreach (Room room in plugin.Scp079Rooms)
				{
					Generator079.generators[0].CallRpcOvercharge();
					room.FlickerLights();
				}

				yield return Timing.WaitForSeconds(11f);
			}
		}

		public IEnumerator<float> SpawnCoin(float delay)
		{
			plugin.Server.Map.Broadcast(10, $"The coin(s) will spawn in 30 seconds! There will be {plugin.CoinCount} coins to collect!", false);
			yield return Timing.WaitForSeconds(delay);
			
			int r = plugin.Gen.Next(plugin.Scp079Rooms.Count);
			
			plugin.Server.Map.SpawnItem(ItemType.COIN, plugin.Scp079Rooms[r].Position, Vector.Zero);
			plugin.Info(plugin.Scp079Rooms[r].Position.ToString());
			plugin.Server.Map.Broadcast(10, "The magic coin(s) has spawned!", false);
		}

		public IEnumerator<float> SpawnClassD(Player player)
		{
			player.ChangeRole(Role.CLASSD, false, false, false);
			player.Teleport(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53));
			yield return Timing.WaitForSeconds(1f);
			
			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();
			player.GiveItem(ItemType.FLASHLIGHT);
		}

		public IEnumerator<float> CheckForWinner(float delay)
		{
			yield return Timing.WaitForSeconds(delay);
			
			for (;;)
			{
				if (!plugin.Winners.Any())
					yield break;
				foreach (Player player in plugin.Server.GetPlayers())
					foreach (Smod2.API.Item item in player.GetInventory())
					if (item.ItemType == ItemType.COIN)
					{
						plugin.Winners.Add(player);
						item.Remove();
						player.ChangeRole(Role.TUTORIAL);
						player.Teleport(new Vector(53, 1020, -44));
						player.PersonalBroadcast(5, "You have found a coin!", false);
					}

				yield return Timing.WaitForSeconds(1f);
			}
		}
		
		public void EnableGamemode()
		{
			plugin.Enabled = true;

			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(25, "<color=#50c878>Finder's Keepers</color> is starting..", false);
			}
		}
		
		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			foreach (CoroutineHandle handle in plugin.Coroutines)
				Timing.KillCoroutines(handle);
		}
	}
}