using System.Reflection;
using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace ZombieSurvival
{
	internal class EventHandler : IEventHandlerWaitingForPlayers, IEventHandlerPlayerJoin, IEventHandlerRoundStart, IEventHandlerRoundRestart, IEventHandlerRoundEnd, IEventHandlerTeamRespawn,
		IEventHandlerPlayerHurt
	{
		private readonly Zombie plugin;
		public EventHandler(Zombie plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled || plugin.RoundStarted) return;

			Server server = plugin.Server;

			server.Map.ClearBroadcasts();
			server.Map.Broadcast(25, "<color=#07A407>Zombie Survival</color> gamemode is starting..", false);
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

			plugin.Info("Zombie Survival round started.");
			PlayerManager.localPlayer.GetComponent<DecontaminationLCZ>().time = 666f;

			Timing.RunCoroutine(plugin.Functions.EndRound());
			Timing.RunCoroutine(plugin.Functions.SpawnAmmo());
			Timing.RunCoroutine(plugin.Functions.SpawnCarePackage());

			List<Player> players = ev.Server.GetPlayers();

			for (int i = 0; i < plugin.MaxNTFCount; i++)
			{
				if (i == players.Count - 1) break;

				int r = plugin.Gen.Next(1, players.Count);
				Timing.RunCoroutine(plugin.Functions.SpawnNTF(players[r]));
				players.RemoveAt(r);
			}

			foreach (Player player in players)
				Timing.RunCoroutine(plugin.Functions.SpawnZombie(player));
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Zombie Survival round restarted.");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Zombie Survival round ended.");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP)
				ev.Damage = (ev.Damage * plugin.ZDamageMultiplier);
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;

			List<Player> respawn = new List<Player>();
			ev.SpawnChaos = true;

			foreach (Player player in ev.PlayerList)
				respawn.Add(player);

			ev.PlayerList = respawn;

			foreach (Player player in ev.PlayerList)
				Timing.RunCoroutine(plugin.Functions.SpawnZombie(player));

			ev.PlayerList.Clear();
		}
	}
}
