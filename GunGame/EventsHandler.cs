using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace Gungame
{
	internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundRestart, IEventHandlerRoundEnd, IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerCheckRoundEnd,
		IEventHandlerThrowGrenade, IEventHandlerPlayerHurt, IEventHandlerWaitingForPlayers
	{
		private readonly GunGame plugin;

		public EventsHandler(GunGame plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				plugin.RoundStarted = true;
				List<Player> players = ev.Server.GetPlayers();

				foreach (Player player in players)
				{
					Timing.RunCoroutine(plugin.Functions.Spawn(player));
					(player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
				}

				string[] dlist = { "914", "GATE_A", "GATE_B" };
				string[] olist = { "CHECKPOINT_ENT", "CHECKPOINT_LCZ_A", "CHECKPOINT__LCZB" };

				foreach (string d in dlist)
					foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
						if (d == door.Name)
						{
							door.Open = false;
							door.Locked = true;
						}

				foreach (string o in olist)
					foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
						if (o == door.Name)
						{
							door.Open = true;
							door.Locked = true;
						}
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.RoundStarted) return;

			(ev.Player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
			Timing.RunCoroutine(plugin.Functions.Spawn(ev.Player));
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			ev.Player.GiveItem(ItemType.FRAG_GRENADE);
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			plugin.Functions.ReplaceGun(ev.Killer);

			if (!plugin.Reversed && ev.DamageTypeVar == DamageType.E11_STANDARD_RIFLE)
			{
				plugin.Functions.AnnounceWinner(ev.Killer);
				plugin.Winner = ev.Killer;
			}
			else if (plugin.Reversed && ev.DamageTypeVar == DamageType.FRAG)
			{
				plugin.Functions.AnnounceWinner(ev.Killer);
				plugin.Winner = ev.Killer;
			}
			else
				Timing.RunCoroutine(plugin.Functions.Spawn(ev.Player));
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			if (ev.Player.SteamId == ev.Attacker.SteamId && ev.DamageType == DamageType.FRAG)
				ev.Damage = 0;

		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			if (!(plugin.Winner is Player))
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			plugin.Functions.EndGamemodeRound();
		}
	}
}