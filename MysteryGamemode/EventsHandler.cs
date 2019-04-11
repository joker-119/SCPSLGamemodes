using System;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using scp4aiur;
using UnityEngine;
using Smod2.Commands;
using System.Linq;

namespace Mystery
{
	public class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers,
		IEventHandlerPlayerDie, IEventHandlerRoundRestart
	{
		private readonly Mystery plugin;
		public EventsHandler(Mystery plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (plugin.Enabled)
			{
				if (!plugin.RoundStarted)
				{
					Server server = plugin.Server;
					server.Map.ClearBroadcasts();
					server.Map.Broadcast(25, "<color=#c50000>Murder Mystery</color> gamemode is starting...", false);
				}
				else
					(ev.Player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
			}
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (plugin.Enabled)
			{
				plugin.RoundStarted = true;
				plugin.Server.Map.ClearBroadcasts();
				plugin.Info("Mystery Gamemode started!");

				List<Player> players = ev.Server.GetPlayers();
				List<string> murds = new List<string>();
				List<string> dets = new List<string>();

				foreach (GameObject player in PlayerManager.singleton.players)
				{
					player.GetComponent<WeaponManager>().NetworkfriendlyFire = true;
				}

				for (int i = 0; i < plugin.MurdererNum; i++)
				{
					if (players.Count == 0) break;
					int random = plugin.gen.Next(players.Count);
					Player ranplayer = players[random];
					players.Remove(ranplayer);
					Timing.Run(plugin.Functions.SpawnMurd(ranplayer));
				}
				for (int i = 0; i < plugin.DetectiveNum; i++)
				{
					if (players.Count == 0) break;
					int random = plugin.gen.Next(players.Count);
					Player ranplayer = players[random];
					players.Remove(ranplayer);
					Timing.Run(plugin.Functions.SpawnDet(ranplayer));
				}
				foreach (Player player in players)
				{
					Timing.Run(plugin.Functions.SpawnCiv(player));
				}
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Ended!");
			plugin.Functions.EndGamemoderound();

		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Restarted.");
			plugin.Functions.EndGamemoderound();
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.TeamRole.Role == Role.CLASSD)
			{
				if (plugin.murd.ContainsKey(ev.Player.SteamId))
				{
					plugin.Server.Map.ClearBroadcasts();
					plugin.Server.Map.Broadcast(15, "A murderer, " + ev.Player.Name + " has been eliminated by " + ev.Killer.Name + "!", false);
					plugin.murd.Remove(ev.Player.SteamId);
				}
				else
				{
					plugin.Server.Map.ClearBroadcasts();
					plugin.Server.Map.Broadcast(25, "There are now " + (plugin.Server.Round.Stats.ClassDAlive - 1) + " Civilians alive.", false);
					if (!plugin.murd.ContainsKey(ev.Killer.SteamId) && (ev.Killer is Player))
					{
						ev.Killer.ChangeRole(Role.SPECTATOR);
						ev.Killer.PersonalClearBroadcasts();
						ev.Killer.PersonalBroadcast(10, "<color=#c50000>You killed an innocent person! You monster!", false);
					}
				}
			}
			else if (ev.Player.TeamRole.Role == Role.SCIENTIST)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(15, "A detective, " + ev.Player.Name + " has been killed!", false);
				if (!plugin.murd.ContainsKey(ev.Killer.SteamId) && (ev.Killer is Player))
				{
					ev.Killer.Kill();
					ev.Player.PersonalClearBroadcasts();
					ev.Player.PersonalBroadcast(10, "<color=#c50000>You were innocent and killed a Detective! How rude!", false);
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
			{
				if (plugin.murd.ContainsKey(player.SteamId))
				{
					murdAlive = true; continue;
				}
				else if (player.TeamRole.Role == Smod2.API.Role.CLASSD)
				{
					civAlive = true;
				}
			}

			if (murdAlive && civAlive)
			{
				ev.Status = ROUND_END_STATUS.ON_GOING;
			}
			else if (!murdAlive && civAlive)
			{
				ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemoderound();
				plugin.Info("All of the murderers are dead!");
			}
			else if (murdAlive && !civAlive)
			{
				ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemoderound();
				plugin.Info("All the Civilians are dead!");
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Myst Respawn.");
			ev.SpawnChaos = true;


			foreach (Player player in ev.PlayerList)
			{
				int random = plugin.gen.Next(ev.PlayerList.Count);
				Player ranmurd = ev.PlayerList[random];
				ev.PlayerList.Remove(ranmurd);
				int Random = plugin.gen.Next(ev.PlayerList.Count);
				Player randet = ev.PlayerList[random];
				ev.PlayerList.Remove(randet);

				Timing.Run(plugin.Functions.SpawnMurd(ranmurd));
				Timing.Run(plugin.Functions.SpawnDet(randet));
			}
			foreach (Player player in ev.PlayerList)
			{
				Timing.Run(plugin.Functions.SpawnCiv(player));
			}

		}
	}
}