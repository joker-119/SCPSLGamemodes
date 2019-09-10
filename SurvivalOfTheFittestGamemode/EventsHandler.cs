using System.Collections.Generic;
using MEC;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace SurvivalGamemode
{
	internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerPlayerTriggerTesla, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerRoundRestart,
		 IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie
	{
		private readonly Survival plugin;

		public EventsHandler(Survival plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!plugin.Enabled) return;

			if (plugin.RoundStarted) return;
			
			Server server = plugin.Server;
			server.Map.ClearBroadcasts();
			server.Map.Broadcast(25, "<color=#50c878>Survival Gamemode</color> is starting...", false);
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
			plugin.Functions.Get079Rooms();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) 
				return;
			
			Timing.RunCoroutine(plugin.Functions.TeleportNuts(plugin.NutDelay));
			Timing.RunCoroutine(plugin.Functions.LczBlackout());
			Timing.RunCoroutine(plugin.Functions.HczBlackout());

			plugin.RoundStarted = true;

			plugin.Server.Map.ClearBroadcasts();
			plugin.Info("Survival of the Fittest Gamemode Started!");

			string[] dList = new string[] { "CHECKPOINT_LCZ_A", "CHECKPOINT_LCZ_B", "CHECKPOINT_ENT", "173", "HCZ_ARMORY", "NUKE_ARMORY", "049_ARMORY" };

			foreach (string d in dList)
			foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
				if (d == door.Name)
				{
					plugin.Info("Locking " + door.Name + ".");
					door.Open = false;
					door.Locked = true;
				}

			string[] oList = new string[] { "HID", "106_BOTTOM", "106_PRIMARY", "106_SECONDARY", "079_SECOND", "079_FIRST", "096" };

			foreach (string o in oList)
			foreach (Smod2.API.Door door in ev.Server.Map.GetDoors())
				if (o == door.Name)
				{
					plugin.Info("Opening " + door.Name + ".");
					door.Open = true;
					door.Locked = true;
				}

			int count = 0;

			foreach (Player player in ev.Server.GetPlayers())
				if (player.TeamRole.Team != Smod2.API.Team.SCP && player.TeamRole.Team != Smod2.API.Team.SPECTATOR)
					plugin.Functions.SpawnDboi(player);
				else if (player.TeamRole.Team == Smod2.API.Team.SCP && count < plugin.NutLimit)
				{
					plugin.Functions.SpawnNut(player);
					count++;
				}
				else
					plugin.Functions.SpawnDboi(player);
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

			if (ev.Player.TeamRole.Role != Role.CLASSD) 
				return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(5, "There are now " + (plugin.Server.Round.Stats.ClassDAlive - 1) + " Class-D remaining.", false);

			ev.Player.PersonalClearBroadcasts();
			string[] lines = new[]
			{
				"Skidaddle, skidacted, your neck is now [REDACTED]!",
				"Skidaddle, skidoodle, your neck is now a noodle!", "Skidaddle, skidoken, your neck is now broken!"
			};
			ev.Player.PersonalBroadcast(5, lines[plugin.Gen.Next(lines.Length)], false);
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool peanutAlive = false;
			bool humanAlive = false;
			int humanCount = 0;

			foreach (Player player in ev.Server.GetPlayers())
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
					peanutAlive = true;

				else if (player.TeamRole.Team != Smod2.API.Team.SCP && player.TeamRole.Team != Smod2.API.Team.SPECTATOR)
				{
					humanAlive = true;
					humanCount++;
				}

			if (ev.Server.GetPlayers().Count <= 1) return;
			
			if (peanutAlive && humanAlive && humanCount > 1)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (peanutAlive && humanAlive && humanCount == 1)
			{
				ev.Status = ROUND_END_STATUS.OTHER_VICTORY; plugin.Functions.EndGamemodeRound();
				foreach (Player player in ev.Server.GetPlayers())
					if (player.TeamRole.Team == Smod2.API.Team.CLASSD)
					{
						ev.Server.Map.ClearBroadcasts();
						ev.Server.Map.Broadcast(10, player.Name + " Winner, winner, chicken dinner!", false);
					}
			}
			else if (peanutAlive && humanAlive == false)
			{
				ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
			}
			else if (peanutAlive == false && humanAlive)
			{
				ev.Status = ROUND_END_STATUS.CI_VICTORY; plugin.Functions.EndGamemodeRound();
			}
		}
		
		public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
		{
			if (!plugin.RoundStarted) return;

			ev.Triggerable = false;
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;
			plugin.Info("Surv Respawn.");

			ev.SpawnChaos = true;
			ev.PlayerList = new List<Player>();
		}
	}
}
