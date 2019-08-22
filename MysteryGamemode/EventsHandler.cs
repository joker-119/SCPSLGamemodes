using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using UnityEngine;

namespace Mystery
{
	public class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart,
		IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie,
		IEventHandlerRoundRestart
	{
		private readonly Mystery plugin;
		public EventsHandler(Mystery plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled) return;
			
			if (!plugin.RoundStarted)
			{
				Server server = plugin.Server;
				server.Map.ClearBroadcasts();
				server.Map.Broadcast(25, "<color=#c50000>Murder Mystery</color> gamemode is starting...", false);
			}
			else
				((GameObject)ev.Player.GetGameObject()).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) return;
			
			plugin.RoundStarted = true;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Info("Mystery Gamemode started!");

			List<Player> players = ev.Server.GetPlayers();

			foreach (GameObject player in PlayerManager.singleton.players) player.GetComponent<WeaponManager>().NetworkfriendlyFire = true;

			for (int i = 0; i < plugin.MurdererNum; i++)
			{
				if (players.Count == 0) break;
				int random = plugin.Gen.Next(players.Count);
				Player player = players[random];
				players.Remove(player);
				Timing.RunCoroutine(plugin.Functions.SpawnMurd(player));
			}
			for (int i = 0; i < plugin.DetectiveNum; i++)
			{
				if (players.Count == 0) break;
				int random = plugin.Gen.Next(players.Count);
				Player player = players[random];
				players.Remove(player);
				Timing.RunCoroutine(plugin.Functions.SpawnDet(player));
			}
			
			foreach (Player player in players) Timing.RunCoroutine(plugin.Functions.SpawnCiv(player));
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

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			switch (ev.Player.TeamRole.Role)
			{
				case Role.CLASSD when plugin.Murd.ContainsKey(ev.Player.SteamId):
					plugin.Server.Map.ClearBroadcasts();
					plugin.Server.Map.Broadcast(15, "A murderer, " + ev.Player.Name + " has been eliminated by " + ev.Killer.Name + "!", false);
					plugin.Murd.Remove(ev.Player.SteamId);
					break;
				case Role.CLASSD:
				{
					plugin.Server.Map.ClearBroadcasts();
					plugin.Server.Map.Broadcast(25, "There are now " + (plugin.Server.Round.Stats.ClassDAlive - 1) + " Civilians alive.", false);
					if (!plugin.Murd.ContainsKey(ev.Killer.SteamId) && ev.Killer != null)
					{
						ev.Killer.ChangeRole(Role.SPECTATOR);
						ev.Killer.PersonalClearBroadcasts();
						ev.Killer.PersonalBroadcast(10, "<color=#c50000>You killed an innocent person! You monster!", false);
					}

					break;
				}
				case Role.SCIENTIST:
				{
					plugin.Server.Map.ClearBroadcasts();
					plugin.Server.Map.Broadcast(15, "A detective, " + ev.Player.Name + " has been killed!", false);
					if (!plugin.Murd.ContainsKey(ev.Killer.SteamId) && ev.Killer != null)
					{
						ev.Killer.Kill();
						ev.Player.PersonalClearBroadcasts();
						ev.Player.PersonalBroadcast(10, "<color=#c50000>You were innocent and killed a Detective! How rude!", false);
					}

					break;
				}
			}
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;
			if (plugin.Server.Round.Duration < 5) return;

			bool civAlive = false;
			bool murdAlive = false;

			foreach (Player player in ev.Server.GetPlayers().Where(ply => ply.TeamRole.Team != Smod2.API.Team.SPECTATOR))
				if (plugin.Murd.ContainsKey(player.SteamId))
					murdAlive = true;
				else if (player.TeamRole.Role == Role.CLASSD && !plugin.Murd.ContainsKey(player.SteamId)) 
					civAlive = true;

			if (murdAlive && civAlive)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (!murdAlive && civAlive)
			{
				ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemodeRound();
				plugin.Info("All of the murderers are dead!");
			}
			else if (murdAlive && !civAlive)
			{
				ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
				plugin.Info("All the Civilians are dead!");
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Myst Respawn.");
			ev.SpawnChaos = true;

			if (plugin.MurdRespawn)
			{
				int r1 = plugin.Gen.Next(ev.PlayerList.Count);
				Timing.RunCoroutine(plugin.Functions.SpawnMurd(ev.PlayerList[r1]));
				ev.PlayerList.RemoveAt(r1);
			}

			if (plugin.DetRespawn)
			{
				int r2 = plugin.Gen.Next(ev.PlayerList.Count);
				Timing.RunCoroutine(plugin.Functions.SpawnDet(ev.PlayerList[r2]));
				ev.PlayerList.RemoveAt(r2);
			}

			foreach (Player player in ev.PlayerList) 
				Timing.RunCoroutine(plugin.Functions.SpawnCiv(player));
		}
	}
}