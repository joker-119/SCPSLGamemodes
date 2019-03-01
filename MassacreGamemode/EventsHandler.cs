using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;

namespace MassacreGamemode
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerSetRole, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie
    {
        private readonly Massacre plugin;
        public static Player winner = null;

        public EventsHandler(Massacre plugin) => this.plugin = plugin;
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (Massacre.enabled)
            {
                if (!Massacre.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=#50c878>Massacre Gamemode</color> is starting...", false);
                }
            }
        }
        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (Massacre.enabled || Massacre.roundstarted)
            {
               if (ev.TeamRole.Role == Role.SCP_173)
               {
                   ev.Player.SetHealth(Massacre.nut_health);
               }
           }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Massacre.SpawnRoom = plugin.GetConfigString("mass_spawn_room");
            Massacre.SpawnLoc = Functions.SpawnLoc();
            Massacre.nut_health = plugin.GetConfigInt("mass_peanut_health");
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Massacre.enabled)
            {
                Massacre.roundstarted = true;
                foreach (Door door in ev.Server.Map.GetDoors())
                {
                    door.Locked = true;
                    door.Open = false;
                }
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Massacre of the D-Bois Gamemode Started!");
                List<Player> players = ev.Server.GetPlayers();
                List<string> nuts = new List<string>();

                for (int i = 0; i < Massacre.nut_count; i++)
                {
                    int random = Massacre.generator.Next(players.Count);
                    string name = players[random].Name;
                    nuts.Add(name);
                }
                foreach (Player player in players)
                {
                    if (nuts.Contains(player.Name))
                    {
                        Functions.SpawnNut(player);
                    }
                    else
                    {
                        Functions.SpawnDboi(player);
                    }
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Massacre.enabled || Massacre.roundstarted)
            {
                plugin.Info("Round Ended!");
                Functions.EndGamemodeRound();
            }
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Massacre.enabled || Massacre.roundstarted)
            {
                if (ev.Player.TeamRole.Role == Role.CLASSD)
                {
                    plugin.Server.Map.ClearBroadcasts();
                    plugin.Server.Map.Broadcast(5, "There are now " + (Massacre.plugin.pluginManager.Server.Round.Stats.ClassDAlive - 1) + " Class-D remaining.", false);
                    ev.Player.PersonalBroadcast(25, "You are dead! But don't worry, now you get to relax and watch your friends die!", false);
                }
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Massacre.enabled || Massacre.roundstarted)
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
            if (Massacre.enabled || Massacre.roundstarted)
            {
                ev.SpawnChaos = true;
                ev.PlayerList = new List<Player>();
            }
        }
    }
}
