using System.Runtime.CompilerServices;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using MEC;
using UnityEngine;

namespace ZombielandGamemode
{
	internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerDoorAccess, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerHurt, IEventHandlerPlayerJoin,
		IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerRoundRestart
	{
		private readonly Zombieland plugin;

		public EventsHandler(Zombieland plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				if (!plugin.RoundStarted)
				{
					Server server = plugin.Server;
					server.Map.ClearBroadcasts();
					server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting...", false);
				}
				else
					(ev.Player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{

			if (GamemodeManager.GamemodeManager.CurrentMode == plugin)
			{
				plugin.RoundStarted = true;

				plugin.Server.Map.ClearBroadcasts();
				plugin.Info("Zombieland Gamemode Started!");

				foreach (Player player in ev.Server.GetPlayers())
				{
					if (player.TeamRole.Team == Smod2.API.Team.SCP)
					{
						Timing.RunCoroutine(plugin.Functions.SpawnAlpha(player));
					}
					(player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
				}

				Timing.RunCoroutine(plugin.Functions.AliveCounter(90));
				Timing.RunCoroutine(plugin.Functions.OpenGates(240));
			}
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (ev.Door.Locked && plugin.Alphas.Contains(ev.Player.SteamId) && plugin.AlphaDoorDestroy)
			{
				ev.Destroy = true;
				ev.Door.Destroyed = true;
				ev.Door.Open = true;
				((Door)ev.Door.GetComponent()).Networkdestroyed = true;
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Ended!");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Restarted.");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool zombieAlive = false;
			bool humanAlive = false;

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					zombieAlive = true; continue;
				}

				else if (player.TeamRole.Team != Smod2.API.Team.SCP && player.TeamRole.Team != Smod2.API.Team.SPECTATOR)
					humanAlive = true;
			}

			if (ev.Server.GetPlayers().Count > 1)
			{
				if (zombieAlive && humanAlive)
				{
					ev.Status = ROUND_END_STATUS.ON_GOING;
				}
				else if (zombieAlive && humanAlive == false)
				{
					ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
				}
				else if (zombieAlive == false && humanAlive)
				{
					ev.Status = ROUND_END_STATUS.CI_VICTORY; plugin.Functions.EndGamemodeRound();
				}
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Attacker.TeamRole.Role == Role.SCP_049_2 && ev.Player != ev.Attacker)
			{
				if (plugin.Alphas.Contains(ev.Attacker.SteamId))
					ev.Damage = plugin.AlphaDamage;
				else
					ev.Damage = plugin.ChildDamage;
			}

			if ((plugin.Enabled || plugin.RoundStarted) && ev.Player.TeamRole.Team != Smod2.API.Team.SCP && ev.Damage >= ev.Player.GetHealth())
			{
				if (ev.Attacker == ev.Player || ev.DamageType == DamageType.TESLA || ev.DamageType == DamageType.NUKE || ev.DamageType == DamageType.LURE || ev.DamageType == DamageType.DECONT)
				{
					ev.Player.ChangeRole(Role.SPECTATOR);
				}
				else
				{
					ev.Damage = 0;
					Timing.RunCoroutine(plugin.Functions.SpawnChild(ev.Player, ev.Attacker));
				}
			}
		}
		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;
			plugin.Info("Zombie Respawn.");

			ev.SpawnChaos = true;
		}
	}
}
