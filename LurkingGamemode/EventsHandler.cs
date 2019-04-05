using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using scp4aiur;

namespace LurkingGamemode
{
	internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerSpawn, IEventHandlerCheckEscape, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers
	{
		public readonly Lurking plugin;

		public EventsHandler(Lurking plugin) => this.plugin = plugin;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (plugin.Enabled)
			{
				if (!plugin.RoundStarted)
				{
					Server server = plugin.Server;

					server.Map.ClearBroadcasts();
					server.Map.Broadcast(25, "<color=#2D2B2B> Lurking in the dark</color> gamemode starting..", false);
				}
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
			plugin.Functions.Get079Rooms();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!plugin.Enabled) return;

			plugin.RoundStarted = true;

			plugin.Info("Lurking in the Dark gamemode started!");

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					for (int i = 0; i < plugin.LarryCount; i++)
					{
						if (player.TeamRole.Role != Role.SCP_106 && player.TeamRole.Role != Role.SCP_939_53 && player.TeamRole.Role != Role.SCP_939_89)
						{
							plugin.Functions.SpawnLarry(player);
						}
					}
					for (int i = 0; i < plugin.DoggoCount; i++)
					{
						if (player.TeamRole.Role != Role.SCP_106 && player.TeamRole.Role != Role.SCP_939_53 && player.TeamRole.Role != Role.SCP_939_89)
						{
							plugin.Functions.SpawnDoggo(player);
						}
					}
				}
				else if (player.TeamRole.Team == Smod2.API.Team.NINETAILFOX || player.TeamRole.Team == Smod2.API.Team.CHAOS_INSURGENCY)
				{
					player.ChangeRole(Role.FACILITY_GUARD, true, true, true, true);
					player.PersonalClearBroadcasts();
					player.PersonalBroadcast(25, "You are a <color=#2D2B2B> Facility Guard</color>, your job is to protect the scientists and get them outside safely.", false);
				}
				else if (player.TeamRole.Team == Smod2.API.Team.CLASSD)
				{
					player.ChangeRole(Role.SCIENTIST, true, true, true, true);
					player.PersonalClearBroadcasts();
					player.PersonalBroadcast(25, "You are a <color=#C3DA30> Scientist</color>, your job is to escape the facility and terminate the SCP's.", false);
				}
			}

			Timing.Run(plugin.Functions.HCZBlackout());
			Timing.Run(plugin.Functions.LCZBlackout());
		}

		public void OnSpawn(PlayerSpawnEvent ev)
		{
			if (!plugin.FlashlightsOnSpawn) return;

			bool hasLight = false;
			foreach (Smod2.API.Item item in ev.Player.GetInventory())
			{
				if (item.ItemType == ItemType.FLASHLIGHT)
				{
					hasLight = true;
				}
			}

			if (!hasLight)
				ev.Player.GiveItem(ItemType.FLASHLIGHT);
		}

		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (ev.AllowEscape)
				plugin.Server.Map.StartWarhead();
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			plugin.Info("Round Ended!");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			bool scpAlive = false;
			bool humanAlive = false;
			int humanCount = 0;

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Smod2.API.Team.SCP)
				{
					scpAlive = true; continue;
				}
				else if (player.TeamRole.Team != Smod2.API.Team.SCP && player.TeamRole.Team != Smod2.API.Team.SPECTATOR && player.TeamRole.Role != Role.FACILITY_GUARD)
				{
					humanAlive = true;
					humanCount = humanCount++;
				}
			}

			if (ev.Server.GetPlayers().Count > 1)
			{
				if (scpAlive && humanAlive)
				{
					ev.Status = ROUND_END_STATUS.ON_GOING;
				}
				else if (scpAlive && humanAlive == false)
				{
					ev.Status = ROUND_END_STATUS.SCP_VICTORY; plugin.Functions.EndGamemodeRound();
				}
				else if (scpAlive == false && humanAlive)
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemodeRound();
				}
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;
			plugin.Info("Lurk respawn.");

			ev.SpawnChaos = false;
			ev.PlayerList = new List<Player>();
		}
	}
}