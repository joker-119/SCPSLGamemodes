using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;

namespace LurkingGamemode
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers
    {
        public readonly Lurking plugin;

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
                Lurking.blackouts = SCP575.SCP575.enabled;
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
                            SCP575.Functions.singleton.DisableBlackouts();
                            plugin.Info("Disabling timed blackouts.");
                            Lurking.blackouts = true;
                        }
                    }
                }

                Lurking.roundstarted = true;
                plugin.Info("Lurking in the Dark gamemode started!");
                SCP575.Functions.singleton.ToggleBlackout();

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Team.SCP)
                    {
                        for (int i = 0; i < Lurking.larry_count; i++)
                        {
                            if (player.TeamRole.Role != Role.SCP_106 && player.TeamRole.Role != Role.SCP_939_53 && player.TeamRole.Role != Role.SCP_939_89)
                            {
                                Functions.SpawnLarry(player);
                            }
                        }
                        for (int i = 0; i < Lurking.doggo_count; i++)
                        {
                            if (player.TeamRole.Role != Role.SCP_106 && player.TeamRole.Role != Role.SCP_939_53 && player.TeamRole.Role != Role.SCP_939_89)
                            {
                                Functions.SpawnDoggo(player);
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
            if (Lurking.enabled || Lurking.roundstarted)
            {
                plugin.Info("Round Ended!");
                Functions.EndGamemodeRound();
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Lurking.enabled || Lurking.roundstarted)
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
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; Functions.EndGamemodeRound();
                    }
                    else if (scpAlive == false && humanAlive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY; Functions.EndGamemodeRound();
                    }
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Lurking.enabled || Lurking.roundstarted)
            {
                ev.SpawnChaos = false;
                ev.PlayerList = new List<Player>();
            }
        }
    }
}