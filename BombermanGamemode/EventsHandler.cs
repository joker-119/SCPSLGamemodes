using System.Collections.Generic;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace Bomber
{
	internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie, IEventHandlerSetRole, IEventHandlerPlayerHurt,
		IEventHandlerCheckRoundEnd, IEventHandlerPlayerJoin, IEventHandlerRoundRestart
	{
		private readonly Bomber plugin;
		public EventsHandler(Bomber plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}
		
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;
			if (plugin.RoundStarted) return;

			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(25, "<color=#c50000>Bomberman Gamemode</color> is starting..", false);
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (GamemodeManager.GamemodeManager.CurrentMode != plugin) return;
			
			plugin.RoundStarted = true;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Functions.GetPlayers();
			plugin.Info("Bomberman Gamemode started!");

			List<Player> players = plugin.Server.GetPlayers();

			if (plugin.SpawnClass != "")
				switch (plugin.SpawnClass.ToLower().Trim())
				{
					case "classd":
						plugin.Info("Class-D spawn selected.");
						foreach (Player player in players) player.ChangeRole(Role.CLASSD, false, true, false);
						break;
					case "sci":
					case "nerd":
					case "scientist":
						plugin.Info("Scientists spawn selected.");
						foreach (Player player in players) player.ChangeRole(Role.SCIENTIST, false, true, false);
						break;
					case "guard":
						plugin.Info("Guard spawn selected.");
						foreach (Player player in players) player.ChangeRole(Role.FACILITY_GUARD, false, true, false);
						break;
					case "ntf":
						plugin.Info("NTF spawn selected.");
						foreach (Player player in players) player.ChangeRole(Role.NTF_COMMANDER, false, true, false);
						break;
					case "chaos":
						plugin.Info("Chaos spawn selected.");
						foreach (Player player in players) player.ChangeRole(Role.CHAOS_INSURGENCY, false, true, false);
						break;
					case "war":
						plugin.Info("Warmode initiated.");
						plugin.Warmode = true;

						List<string> nerds = new List<string>();
						int num = players.Count / 2;

						if (num == 0)
							plugin.Error("Ya dun goofed, kid!");

						for (int i = 0; i < num; i++)
						{
							int ran = plugin.Gen.Next(players.Count);
							nerds.Add(players[ran].SteamId);
							players.Remove(players[ran]);
						}

						foreach (Player player in players)
							player.ChangeRole(nerds.Contains(player.SteamId) ? Role.SCIENTIST : Role.CLASSD, false, true, false);
						break;
					default:
						plugin.Info("Normal round selected.");
						break;
				}
			
			float delay = plugin.Gen.Next(plugin.Min, plugin.Max);
			Timing.RunCoroutine(plugin.Functions.SpawnGrenades(delay));

			if (plugin.Medkits)
				Timing.RunCoroutine(plugin.Functions.GiveMedkits());
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (plugin.Warmode)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(15, "Half of you are Class-D, the other half are nerds. Yor goal is to kill the opposing team with your grenades!", false);
			}
			else
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(15, "Grenades will start spawning under you shortly. Survive the rain of fire!", false);
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Ended.");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Restarted.");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.SteamId == ev.Attacker.SteamId && plugin.Warmode)
				ev.Damage = 0;

			ev.Damage = ev.Damage * plugin.GrenadeMulti;
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.RoundStarted) return;
			if (ev.Player.Name == "Sever" || ev.Player.Name == "") return;

			List<Player> players = plugin.Server.GetPlayers();

			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(10, "There are " + (players.Count - 1) + " players alive!", false);
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			bool classdAlive = false;
			bool sciAlive = false;
			int aliveCount = 0;

			foreach (Player player in ev.Server.GetPlayers())
			{
				switch (player.TeamRole.Team)
				{
					case Team.CLASSD:
						classdAlive = true;
						continue;
					case Team.SCIENTIST:
						sciAlive = true;
						break;
				}

				if (Functions.IsAlive(player)) aliveCount++;
			}

			if (ev.Server.GetPlayers().Count < 1) return;

			if (plugin.Warmode)
			{
				if (classdAlive && sciAlive)
					ev.Status = ROUND_END_STATUS.ON_GOING;
				else if (classdAlive && !sciAlive)
				{
					ev.Status = ROUND_END_STATUS.CI_VICTORY; plugin.Functions.EndGamemodeRound();
				}
				else if (!classdAlive && sciAlive)
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemodeRound();
				}
			}
			else if (aliveCount > 1)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (aliveCount == 1)
			{
				ev.Status = ROUND_END_STATUS.NO_VICTORY; plugin.Functions.EndGamemodeRound();
				foreach (Player player in ev.Server.GetPlayers())
					if (player.TeamRole.Team != Team.SPECTATOR)
					{
						ev.Server.Map.ClearBroadcasts();
						ev.Server.Map.Broadcast(10, player.Name + " Winner, winner, chicken.. Oh you still exploded.", false);
					}
			}
		}
	}
}
