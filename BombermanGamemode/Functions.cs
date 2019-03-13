using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System;

namespace Bomber
{
	public class Functions
	{
		public static Functions singleton;
		public Bomber Bomber;
		public Functions(Bomber plugin)
		{
			this.Bomber = plugin;
			Functions.singleton = this;
		}

		public void EnableGamemode()
		{
			Bomber.enabled = true;
			if (!Bomber.roundstarted)
			{
				Bomber.Server.Map.ClearBroadcasts();
				Bomber.Server.Map.Broadcast(25, "<color=#c50000>Bomberman Gamemode</color> is starting..", false);
			}
		}
		public void DisableGamemode()
		{
			Bomber.enabled = false;
			Bomber.Server.Map.ClearBroadcasts();
		}
		public void EndGamemodeRound()
		{
			Bomber.Info("EndgamemodeRound");
			Bomber.roundstarted = false;
			Bomber.Server.Round.EndRound();
			Bomber.warmode = false;
		}
		public bool IsAlive(Player player)
		{
			if (player.TeamRole.Team != Team.SPECTATOR)
				return true;
			return false;
		}
		public void DropGrenades()
		{
			Bomber.Info("Dropping grenades.");
			foreach (Player player in Bomber.players)
			{
				if (IsAlive(player))
					player.ThrowGrenade(ItemType.FRAG_GRENADE, false, new Vector(0f, 0f, 0f), true, player.GetPosition(), true, 0f, false);
			}
		}
		public void DropFlash()
		{
			Bomber.Info("Dropping flash.");
			foreach (Player player in Bomber.players)
			{
				if (IsAlive(player))
					player.ThrowGrenade(ItemType.FLASHBANG, false, new Vector(0f, 0f, 0f), true, player.GetPosition(), true, 0f, false);
			}
		}
		public void GetPlayers()
		{
			Bomber.players = Bomber.Server.GetPlayers();
		}
		public IEnumerable<float> GiveMedkits()
		{
			if (!Bomber.medkits) yield break;
			Bomber.Info("Giving medkits!");
			yield return 5;
			List<Player> players = Bomber.Server.GetPlayers();
			foreach (Player player in players)
				player.GiveItem(ItemType.MEDKIT);

		}
		public IEnumerable<float> SpawnGrenades(float delay)
		{
			yield return delay;
			while (Bomber.enabled || Bomber.roundstarted)
			{
				int ran = Bomber.gen.Next(1,100);
				if (ran > 50)
					
				yield return 0.5f;
				for (int i = 0; i < Bomber.count; i++)
				{
					DropGrenades();
					yield return 0.5f;
				}
				Bomber.count++;
				Bomber.timer++;
				int min = 15 - ((Bomber.timer * 2)*2);
				int max = 30 - ((Bomber.timer * 2)*2);
				if (min < 1)
					min = 1;
				if (max < 1)
					max = 2;
				yield return GetTimer(min,max);
			}
		}
		public void BossWave()
		{
			Bomber.timer = (Bomber.timer / 2);
		}
		public float GetTimer(int min, int max)
		{
			List<Player> players = Bomber.Server.GetPlayers();
			List<Player> alive = new List<Player>();
			foreach (Player player in players)
			{
				if (IsAlive(player))
					alive.Add(player);
			}
			if (alive.Count > 2)
				return Bomber.gen.Next(min, (max + 1));
			else if (alive.Count == 2)
				return Bomber.gen.Next((min / 2), ((max + 1)/ 2));
			return 20;
		}
	}
}