using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace Bomber
{
	public class Functions
	{
		private readonly Bomber plugin;
		public Functions(Bomber plugin) => this.plugin = plugin;

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
				plugin.Server.Map.Broadcast(25, "<color=#00ffff> Bomberman Gamemode is starting..</color>", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			plugin.Warmode = false;
		}

		public static bool IsAlive(Player player) => player.TeamRole.Team != Team.SPECTATOR;

		public void DropGrenades()
		{
			plugin.Info("Dropping grenades.");
			foreach (Player player in plugin.Players)
				if (IsAlive(player))
					player.ThrowGrenade(GrenadeType.FRAG_GRENADE, false, new Vector(0f, 0f, 0f), true, player.GetPosition(), true, 0f);
		}

		public void DropFlash()
		{
			plugin.Info("Dropping flash.");
			foreach (Player player in plugin.Players)
				if (IsAlive(player))
					player.ThrowGrenade(GrenadeType.FLASHBANG, false, new Vector(0f, 0f, 0f), true, player.GetPosition(), true, 0f);
		}

		public void GetPlayers()
		{
			plugin.Players = plugin.Server.GetPlayers();
		}

		public IEnumerator<float> GiveMedkits()
		{
			if (!plugin.Medkits) yield break;

			plugin.Info("Giving medkits!");
			yield return Timing.WaitForSeconds(5);

			List<Player> players = plugin.Server.GetPlayers();

			foreach (Player player in players)
				player.GiveItem(ItemType.MEDKIT);

		}

		public IEnumerator<float> SpawnGrenades(float delay)
		{
			yield return Timing.WaitForSeconds(delay);

			while (plugin.RoundStarted)
			{
				int ran = plugin.Gen.Next(1, 100);
				if (ran > 50)

					yield return Timing.WaitForSeconds(0.5f);
				for (int i = 0; i < plugin.Count; i++)
				{
					DropGrenades();
					yield return Timing.WaitForSeconds(0.5f);
				}
				plugin.Count++;
				plugin.Timer++;
				int min = 15 - plugin.Timer * 2 * 2;
				int max = 30 - plugin.Timer * 2 * 2;
				if (min < 1)
					min = 1;
				if (max < 1)
					max = 2;
				yield return Timing.WaitForSeconds(GetTimer(min, max));
			}
		}

		public void BossWave()
		{
			plugin.Timer = plugin.Timer / 2;
		}

		private float GetTimer(int min, int max)
		{
			List<Player> players = plugin.Server.GetPlayers();
			List<Player> alive = new List<Player>();

			foreach (Player player in players)
				if (IsAlive(player))
					alive.Add(player);

			if (alive.Count > 2)
				return plugin.Gen.Next(min, max + 1);
			return alive.Count == 2 ? plugin.Gen.Next(min / 2, (max + 1) / 2) : 20;
		}
	}
}