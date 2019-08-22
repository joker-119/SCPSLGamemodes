using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using UnityEngine;

namespace OutbreakGamemode
{
	internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerDoorAccess, IEventHandlerCheckRoundEnd,
		IEventHandlerRoundStart, IEventHandlerPlayerHurt, IEventHandlerPlayerJoin, IEventHandlerRoundEnd,
		IEventHandlerWaitingForPlayers, IEventHandlerRoundRestart
	{
		private readonly Outbreak plugin;

		public EventsHandler(Outbreak plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled) return;
			
			if (!plugin.RoundStarted)
			{
				Server server = plugin.Server;
				server.Map.ClearBroadcasts();
				server.Map.Broadcast(25, "<color=#50c878>Outbreak Gamemode</color> is starting...", false);
			}
			else
				((GameObject) ev.Player.GetGameObject()).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) return;
			
			plugin.RoundStarted = true;

			plugin.Server.Map.ClearBroadcasts();
			plugin.Info("Outbreak Gamemode Started!");

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Smod2.API.Team.SCP) Timing.RunCoroutine(plugin.Functions.SpawnAlpha(player));
				((GameObject) player.GetGameObject()).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
			}

			Timing.RunCoroutine(plugin.Functions.AliveCounter(90));
			Timing.RunCoroutine(plugin.Functions.OpenGates(240));
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (!ev.Door.Locked || !plugin.Alphas.Contains(ev.Player.SteamId) || !plugin.AlphaDoorDestroy || !plugin.RoundStarted) return;
			
			ev.Destroy = true;
			ev.Door.Destroyed = true;
			ev.Door.Open = true;
			((Door)ev.Door.GetComponent()).Networkdestroyed = true;
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
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
					zombieAlive = true;

				else if (player.TeamRole.Team != Smod2.API.Team.SCP && player.TeamRole.Team != Smod2.API.Team.SPECTATOR)
					humanAlive = true;

			if (ev.Server.GetPlayers().Count <= 1) return;
			
			if (zombieAlive && humanAlive)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (zombieAlive && humanAlive == false)
			{
				ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
			}
			else if (zombieAlive == false && humanAlive)
			{
				ev.Status = ROUND_END_STATUS.CI_VICTORY; plugin.Functions.EndGamemodeRound();
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Attacker.TeamRole.Role == Role.SCP_049_2 && ev.Player != ev.Attacker) 
				ev.Damage = plugin.Alphas.Contains(ev.Attacker.SteamId) ? plugin.AlphaDamage : plugin.ChildDamage;

			if (!plugin.RoundStarted || ev.Player.TeamRole.Team == Smod2.API.Team.SCP ||
			    !(ev.Damage >= ev.Player.GetHealth())) return;
			
			if (ev.Attacker == ev.Player || ev.DamageType == DamageType.TESLA || ev.DamageType == DamageType.NUKE || ev.DamageType == DamageType.LURE || ev.DamageType == DamageType.DECONT)
				ev.Player.ChangeRole(Role.SPECTATOR);
			else
			{
				ev.Damage = 0;
				Timing.RunCoroutine(plugin.Functions.SpawnChild(ev.Player, ev.Attacker));
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
