using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using System;
using scp4aiur;

namespace SurvivalGamemode
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerSetRole, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie
    {
        private readonly Survival plugin;
        public static Player winner = null;

        public EventsHandler(Survival plugin) => this.plugin = plugin;
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (Survival.enabled)
            {
                if (!Survival.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=#50c878>Survival Gamemode</color> is starting...", false);
                }
            }
        }
        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (Survival.enabled || Survival.roundstarted)
            {
               if (ev.TeamRole.Role == Role.SCP_173)
               {
                   ev.Player.SetHealth(Survival.nut_health);
               }
           }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Survival.nut_delay = this.plugin.GetConfigFloat("survival_peanut_delay");
            Survival.nut_health = this.plugin.GetConfigInt("survival_peanut_health");
            Survival.pos = Functions.NutSpawn();
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Survival.enabled)
            {

                foreach (Smod2.Plugin p in PluginManager.Manager.EnabledPlugins)
                {
                    if (p.Details.id == "joker.SCP575" && p is SCP575.SCP575)
                    {
                        if (SCP575.SCP575.Timed)
                        {
                            SCP575.Functions.singleton.DisableBlackouts();
                            plugin.Info("Disabling timed blackouts.");
                            Survival.blackouts = true;
                        }
                    }
                }
                Timing.Run(Functions.TeleportNuts(Survival.nut_delay));
                plugin.Info("Timer Initialized..");
                plugin.Info("Timer set to " + Survival.nut_delay + "s.");

                Survival.roundstarted = true;
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Survival of the Fittest Gamemode Started!");

                string[] dlist = new string[] { "CHECKPOINT_LCZ_A", "CHECKPOINT_LCZ_B", "CHECKPOINT_ENT", "173", "HCZ_ARMORY", "NUKE_ARMORY", "049_ARMORY" };
                
                foreach (string d in dlist)
                {
                    foreach (Door door in ev.Server.Map.GetDoors())
                    {
                        if ( d == door.Name)
                        {
                            plugin.Info("Locking " + door.Name + ".");
                            door.Open = false;
                            door.Locked = true;
                        }
                    }
                }

                string[] olist = new string[] { "HID", "106_BOTTOM", "106_PRIMARY", "106_SECONDARY", "079_SECOND", "079_FIRST", "096" };

                foreach (string o in olist)
                {
                    foreach (Door door in ev.Server.Map.GetDoors())
                    {
                        if (o == door.Name)
                        {
                            plugin.Info("Opening " + door.Name + ".");
                            door.Open = true;
                            door.Locked = true;
                        }
                    }
                }


                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team != Team.SCP && player.TeamRole.Team != Team.SPECTATOR && player != winner)
                    {
                        Functions.SpawnDboi(player);
                    }
                    else if (player.TeamRole.Team == Team.SCP || (player == winner && winner is Player))
                    {
                        Functions.SpawnNut(player);
                    }
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Survival.enabled || Survival.roundstarted)
            {
                plugin.Info("Round Ended!");
                Functions.EndGamemodeRound();
				SCP575.Functions.singleton.ToggleBlackout();
            }
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Survival.enabled || Survival.roundstarted)
            {
                if (ev.Player.TeamRole.Role == Role.CLASSD)
                {
                    plugin.Server.Map.ClearBroadcasts();
					ev.Player.PersonalClearBroadcasts();
					ev.Player.PersonalBroadcast(5, "Skiddaddle, skidacted, your neck is now [REDACTED]!", false);
                    plugin.Server.Map.Broadcast(5, "There are now " + (Survival.plugin.pluginManager.Server.Round.Stats.ClassDAlive - 1) + " Class-D remaining.", false);
                }
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Survival.enabled || Survival.roundstarted)
            {
                bool peanutAlive = false;
                bool humanAlive = false;
                int humanCount = 0;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Team.SCP)
                    {
                        peanutAlive = true; continue;
                    }

                    else if (player.TeamRole.Team != Team.SCP && player.TeamRole.Team != Team.SPECTATOR)
                    {
                        humanAlive = true;
                        humanCount++;
                    }

                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (peanutAlive && humanAlive && humanCount > 1)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (peanutAlive && humanAlive && humanCount == 1)
                    {
                        ev.Status = ROUND_END_STATUS.OTHER_VICTORY; Functions.EndGamemodeRound();
                        foreach (Player player in ev.Server.GetPlayers())
                        {
                            if (player.TeamRole.Team == Team.CLASSD)
                            {
                                ev.Server.Map.ClearBroadcasts();
                                ev.Server.Map.Broadcast(10, player.Name + " Winner, winner, chicken dinner!", false);
                                winner = player;
                            }
                        }
                    }
                    else if (peanutAlive && humanAlive == false)
                    {
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; Functions.EndGamemodeRound();
                    }
                    else if (peanutAlive == false && humanAlive)
                    {
                        ev.Status = ROUND_END_STATUS.CI_VICTORY; Functions.EndGamemodeRound();
                    }
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Survival.enabled || Survival.roundstarted)
            {
                ev.SpawnChaos = true;
                ev.PlayerList = new List<Player>();
            }
        }
    }
}
