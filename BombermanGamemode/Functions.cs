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
			foreach (Player player in Bomber.players)
			{
				if (IsAlive(player))
					player.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, player.GetPosition(), false, 0f, false);
			}
		}
		public void DropFlash()
		{
			for (int i = 0; i < Bomber.count; i++)
			{
				foreach (Player player in Bomber.players)
				{
					if (IsAlive(player))
						player.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, player.GetPosition(), false, 0f, false);
				}
			}
		}
		public IEnumerable<float> GiveMedkits()
		{
			if (!Bomber.medkits) yield break;
			Bomber.Info("Giving medkits!");
			yield return 5;
			foreach (Player player in Bomber.players)
				player.GiveItem(ItemType.MEDKIT);

		}
		public IEnumerable<float> SpawnGrenades(float delay)
		{
			yield return delay;
			while (Bomber.enabled || Bomber.roundstarted)
			{
				Bomber.Info("Starting grenade loop.");
				int ran = Bomber.gen.Next(1,100);
				if (ran > 50)
					DropFlash();
				yield return 0.5f;
				for (int i = 0; i < Bomber.count; i++)
				{
					foreach (Player player in Bomber.players)
					{
						if (IsAlive(player))
						{
							player.ThrowGrenade(ItemType.FRAG_GRENADE, false, Vector.Zero, false, player.GetPosition(), false, 0f, false);
							Bomber.Info("Dropping a grenade on " + player.Name);
						}
					}
					yield return 0.75f;
				}
				Bomber.count++;
				Bomber.timer++;
				int min = 15 - ((Bomber.timer * 2)*2);
				int max = 30 - ((Bomber.timer * 2)*2);
				if (min < 0)
					min = 1;
				if (max < 0)
					max = 1;
				Bomber.Info("Timer set to: " + GetTimer(min,max));
				yield return GetTimer(min,max);
			}
		}
		public void BossWave()
		{
			Bomber.timer = (Bomber.timer / 2);
		}
		public float GetTimer(int min, int max)
		{
			if (Bomber.players.Count > 2)
				return Bomber.gen.Next(min, (max + 1));
			else if (Bomber.players.Count == 2)
				return Bomber.gen.Next((min / 2), ((max + 1)/ 2));
			return 20;
		}
	}
}