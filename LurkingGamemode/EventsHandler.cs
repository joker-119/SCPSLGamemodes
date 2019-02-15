using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using System;
using System.Timers;

namespace LurkingGamemode
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers
    {
        public readonly Lurking plugin;
        public static bool blackouts;

        public EventsHandler(Lurking plugin) => this.plugin = plugin;

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (Lurking.enabled)
            {
                if (!Lurking.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=#2D2B2B> Lurking in the dark</color> gamemode starting..", false);
                }
                blackouts = SCP575.SCP575.enabled;
            }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Lurking.larry_count = this.plugin.GetConfigInt("lurking_106_num");
            Lurking.doggo_count = this.plugin.GetConfigInt("lurking_939_num");
            Lurking.larry_health = this.plugin.GetConfigInt("lurking_106_health");
            Lurking.doggo_health = this.plugin.GetConfigInt("lurking_939_health");

        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Lurking.enabled)
            {
                foreach (Smod2.Plugin p in PluginManager.Manager.EnabledPlugins)
                {
                    if (p.Details.id == "Blackout" && p is SCP575.SCP575)
                    {
                        if (SCP575.SCP575.enabled)
                        {
                            SCP575.Functions.DisableBlackouts();
                            plugin.Info("Disabling timed blackouts.");
                            blackouts = true;
                        }
                    }
                }

                Lurking.roundstarted = true;
                plugin.Info("Lurking in the Dark gamemode started!");
                SCP575.Functions.ToggleBlackout();

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Team.SCP)
                    {
                        for (int i = 0; i < Lurking.larry_count; i++)
                        {
                            if (player.TeamRole.Role != Role.SCP_106 && player.TeamRole.Role != Role.SCP_939_53 && player.TeamRole.Role != Role.SCP_939_89)
                            {
                                SpawnLarry(player);
                            }
                        }
                        for (int i = 0; i < Lurking.doggo_count; i++)
                        {
                            if (player.TeamRole.Role != Role.SCP_106 && player.TeamRole.Role != Role.SCP_939_53 && player.TeamRole.Role != Role.SCP_939_89)
                            {
                                SpawnDoggo(player);
                            }
                        }
                    }
                    else if (player.TeamRole.Team == Team.NINETAILFOX || player.TeamRole.Team == Team.CHAOS_INSURGENCY)
                    {
                        player.ChangeRole(Role.FACILITY_GUARD, true, true, true, true);
                        player.PersonalClearBroadcasts();
                        player.PersonalBroadcast(25, "You are a <color=#2D2B2B> Facility Guard</color>, your job is to protect the scientists and get them outside safely.", false);
                    }
                    else if (player.TeamRole.Team == Team.CLASSD)
                    {
                        player.ChangeRole(Role.SCIENTIST, true, true, true, true);
                        player.PersonalClearBroadcasts();
                        player.PersonalBroadcast(25, "You are a <color=#C3DA30> Scientist</color>, your job is to escape the facility and terminate the SCP's.", false);
                    }
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Lurking.enabled)
            {
                plugin.Info("Round Ended!");
                EndGamemodeRound();
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Lurking.enabled)
            {
                bool scpAlive = false;
                bool humanAlive = false;
                int humanCount = 0;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Team.SCP)
                    {
                        scpAlive = true; continue;
                    }
                    else if (player.TeamRole.Team != Team.SCP && player.TeamRole.Team != Team.SPECTATOR && player.TeamRole.Role != Role.FACILITY_GUARD)
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
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; EndGamemodeRound();
                    }
                    else if (scpAlive == false && humanAlive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY; EndGamemodeRound();
                    }
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Lurking.enabled)
            {
                ev.SpawnChaos = false;
                ev.PlayerList = new List<Player>();
            }
        }

        public void EndGamemodeRound()
        {
            if (Lurking.enabled)
            {
                plugin.Info("EndgameRound Function");
                Lurking.roundstarted = false;
                plugin.Server.Round.EndRound();

                if (blackouts)
                {
                    SCP575.Functions.EnableBlackouts();
                    plugin.Info("Enabling timed Blackouts.");
                }
                SCP575.Functions.ToggleBlackout();
            }
        }

        public void SpawnLarry(Player player)
        {
            player.ChangeRole(Role.SCP_106, false, true, false, false);
            player.SetHealth(Lurking.larry_health);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
        }
        public void SpawnDoggo(Player player)
        {
            if (player.TeamRole.Role != Role.SCP_106)
            {
                player.ChangeRole(Role.SCP_939_53, false, true, false, false);
                player.SetHealth(Lurking.doggo_health);
                player.PersonalClearBroadcasts();
                player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
            }
        }
    }
}