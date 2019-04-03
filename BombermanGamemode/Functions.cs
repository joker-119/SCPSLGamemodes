using System.Linq;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System;
using Smod2.Commands;

namespace Bomber
{
	public class Functions
	{
		public Bomber plugin;
		public Functions(Bomber plugin) => this.plugin = plugin;

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
				plugin.Server.Map.Broadcast(25, "<color=#c50000>Bomberman Gamemode</color> is starting..", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.Info("EndgamemodeRound");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
			plugin.Warmode = false;
		}

		public bool IsAlive(Player player)
		{
			if (player.TeamRole.Team != Team.SPECTATOR)
				return true;
			return false;
		}

		public void DropGrenades()
		{
			plugin.Info("Dropping grenades.");
			foreach (Player player in plugin.players)
			{
				if (IsAlive(player))
					player.ThrowGrenade(ItemType.FRAG_GRENADE, false, new Vector(0f, 0f, 0f), true, player.GetPosition(), true, 0f, false);
			}
		}

		public void DropFlash()
		{
			plugin.Info("Dropping flash.");
			foreach (Player player in plugin.players)
			{
				if (IsAlive(player))
					player.ThrowGrenade(ItemType.FLASHBANG, false, new Vector(0f, 0f, 0f), true, player.GetPosition(), true, 0f, false);
			}
		}

		public void GetPlayers()
		{
			plugin.players = plugin.Server.GetPlayers();
		}

		public IEnumerable<float> GiveMedkits()
		{
			if (!plugin.Medkits) yield break;

			plugin.Info("Giving medkits!");
			yield return 5;

			List<Player> players = plugin.Server.GetPlayers();

			foreach (Player player in players)
				player.GiveItem(ItemType.MEDKIT);

		}

		public IEnumerable<float> SpawnGrenades(float delay)
		{
			yield return delay;

			while (plugin.Enabled || plugin.RoundStarted)
			{
				int ran = plugin.Gen.Next(1, 100);
				if (ran > 50)

					yield return 0.5f;
				for (int i = 0; i < plugin.Count; i++)
				{
					DropGrenades();
					yield return 0.5f;
				}
				plugin.Count++;
				plugin.Timer++;
				int min = 15 - ((plugin.Timer * 2) * 2);
				int max = 30 - ((plugin.Timer * 2) * 2);
				if (min < 1)
					min = 1;
				if (max < 1)
					max = 2;
				yield return GetTimer(min, max);
			}
		}

		public void BossWave()
		{
			plugin.Timer = (plugin.Timer / 2);
		}

		public float GetTimer(int min, int max)
		{
			List<Player> players = plugin.Server.GetPlayers();
			List<Player> alive = new List<Player>();

			foreach (Player player in players)
			{
				if (IsAlive(player))
					alive.Add(player);
			}

			if (alive.Count > 2)
				return plugin.Gen.Next(min, (max + 1));
			else if (alive.Count == 2)
				return plugin.Gen.Next((min / 2), ((max + 1) / 2));
			return 20;
		}
	}
}