using Smod2;
using UnityEngine;
using Smod2.API;
using Smod2.Commands;
using System.Linq;
using System.Collections.Generic;


namespace ZombielandGamemode
{
	public class Methods
	{
		private readonly Zombieland plugin;
		public Methods(Zombieland plugin) => this.plugin = plugin;

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
				plugin.Server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting..", false);
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
			plugin.CommandManager.CallCommand(null, "SETCONFIG", new string[] { "friendly_fire", "false" });
		}

		public IEnumerable<float> SpawnChild(Player player, Player killer)
		{
			Vector spawn = player.GetPosition();

			player.ChangeRole(Role.SCP_049_2, false, false, false, false);
			yield return 2;

			player.SetHealth(plugin.ChildHealth);

			player.Teleport(spawn);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, killer.Name + " killed you, and you became a <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
		}

		public IEnumerable<float> AliveCounter(float delay)
		{
			while (plugin.Enabled || plugin.RoundStarted)
			{
				int human_count = (plugin.Round.Stats.NTFAlive + plugin.Round.Stats.ScientistsAlive + plugin.Round.Stats.ClassDAlive + plugin.Round.Stats.CiAlive);

				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(10, "There are currently " + plugin.Round.Stats.Zombies + " zombies and " + human_count + " humans alive.", false);
				yield return delay;
			}
		}
		public IEnumerable<float> SpawnAlpha(Player player)
		{
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);

			player.ChangeRole(Role.SCP_049_2, false, false, false, false);
			yield return 2;

			player.Teleport(spawn);

			plugin.Alphas.Add(player.SteamId);

			player.SetHealth(plugin.AlphaHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are an alpha <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
		}

		public IEnumerable<float> OpenGates(float delay)
		{
			yield return delay;

			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors())
			{
				if (door.Name == "GATE_A" || door.Name == "GATE_B")
				{
					door.Open = true;
					door.Locked = true;
				}
			}
		}
	}
}