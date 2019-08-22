using System.Collections.Generic;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using UnityEngine;

namespace Gungame
{
	internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundRestart, IEventHandlerRoundEnd, IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerCheckRoundEnd,
		IEventHandlerThrowGrenade, IEventHandlerPlayerHurt, IEventHandlerWaitingForPlayers, IEventHandlerTeamRespawn
	{
		private readonly GunGame plugin;

		public EventsHandler(GunGame plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}
		
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled) return;

			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(25, "<color=#123456>GunGame gamemode is starting...</color>", false);
			}
			else
				((GameObject) ev.Player.GetGameObject()).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) return;
			
			plugin.RoundStarted = true;
			List<Player> players = ev.Server.GetPlayers();

			foreach (Player player in players)
			{
				Timing.RunCoroutine(plugin.Functions.Spawn(player));
				((GameObject) player.GetGameObject()).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
			}

			string[] dList = { "914", "GATE_A", "GATE_B" };
			string[] oList = { "CHECKPOINT_ENT", "CHECKPOINT_LCZ_A", "CHECKPOINT__LCZ_B" };

			foreach (string d in dList)
			foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
				if (d == door.Name)
				{
					door.Open = false;
					door.Locked = true;
				}

			foreach (string o in oList)
			foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
				if (o == door.Name)
				{
					door.Open = true;
					door.Locked = true;
				}
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (!plugin.RoundStarted) return;

			ev.Player.GiveItem(ItemType.FRAG_GRENADE);
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Functions.ReplaceGun(ev.Killer);
			plugin.Info("Replaced gun.");

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
			{
				Timing.RunCoroutine(plugin.Functions.Spawn(ev.Player));
				plugin.Info("Spawned player.");
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.SteamId == ev.Attacker.SteamId && ev.DamageType == DamageType.FRAG)
				ev.Damage = 0;

		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;

			ev.SpawnChaos = true;
			ev.PlayerList = new List<Player>();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			if (plugin.Winner == null)
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Functions.EndGamemodeRound();
		}
	}
}