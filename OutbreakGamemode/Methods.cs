using System.Collections.Generic;
using MEC;
using Smod2.API;

namespace OutbreakGamemode
{
	public class Methods
	{
		private readonly Outbreak plugin;
		public Methods(Outbreak plugin) => this.plugin = plugin;

/*
		public bool IsAllowed(ICommandSender sender)
		{
			Player player = sender as Player;

			if (player != null)
			{
				List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0 ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

				if (roleList != null && roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
					return true;
				if (roleList == null || roleList.Count == 0)
					return true;
				return false;
			}
			return true;
		}
*/

/*
		public void EnableGamemode()
		{
			plugin.Enabled = true;

			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting..", false);
			}
		}
*/
/*
		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}
*/

		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}

		public IEnumerator<float> SpawnChild(Player player, Player killer)
		{
			Vector spawn = player.GetPosition();

			player.ChangeRole(Role.SCP_049_2, false, false, false);
			yield return Timing.WaitForSeconds(2);

			player.SetHealth(plugin.ChildHealth);

			player.Teleport(spawn);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, killer.Name + " killed you, and you became a <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
		}

		public IEnumerator<float> AliveCounter(float delay)
		{
			while (plugin.RoundStarted)
			{
				int humanCount = plugin.Round.Stats.NTFAlive + plugin.Round.Stats.ScientistsAlive + plugin.Round.Stats.ClassDAlive + plugin.Round.Stats.CiAlive;

				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(10, "There are currently " + plugin.Round.Stats.Zombies + " zombies and " + humanCount + " humans alive.", false);
				yield return Timing.WaitForSeconds(delay);
			}
		}
		public IEnumerator<float> SpawnAlpha(Player player)
		{
			Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);

			player.ChangeRole(Role.SCP_049_2, false, false, false);
			yield return Timing.WaitForSeconds(2);

			player.Teleport(spawn);

			plugin.Alphas.Add(player.SteamId);

			player.SetHealth(plugin.AlphaHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are an alpha <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
		}

		public IEnumerator<float> OpenGates(float delay)
		{
			yield return Timing.WaitForSeconds(delay);

			foreach (Smod2.API.Door door in plugin.Server.Map.GetDoors())
				if (door.Name == "GATE_A" || door.Name == "GATE_B")
				{
					door.Open = true;
					door.Locked = true;
				}
		}
	}
}