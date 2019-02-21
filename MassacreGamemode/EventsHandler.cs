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
        public static Vector SpawnLoc;

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
            if (Massacre.enabled)
            {
               if (ev.TeamRole.Team == Team.SCP && ev.TeamRole.Role != Role.SCP_173)
               {
                   SpawnNut(ev.Player);
               }
               else if (ev.TeamRole.Team != Team.SPECTATOR && ev.TeamRole.Team != Team.SCP)
               {
                    SpawnDboi(ev.Player);
               }
               else if (ev.TeamRole.Team == Team.SPECTATOR)
               {
                    ev.Player.PersonalBroadcast(25, "You are dead! But don't worry, now you get to relax and watch your friends die!", false);
               }
           }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Massacre.SpawnRoom = plugin.GetConfigString("mass_spawn_room");
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Massacre.enabled)
            {
                Massacre.roundstarted = true;
                SpawnLoc = Functions.SpawnLoc();
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Massacre of the D-Bois Gamemode Started!");

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team != Team.SCP && player.TeamRole.Team != Team.SPECTATOR && player != winner)
                    {
                        SpawnDboi(player);
                    }
                    else if (player.TeamRole.Team == Team.SCP || player == winner)
                    {
                        SpawnNut(player);
                    }
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Massacre.enabled)
            {
                plugin.Info("Round Ended!");
                EndGamemodeRound();
            }
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Massacre.enabled)
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
            if (Massacre.enabled)
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
                        ev.Status = ROUND_END_STATUS.OTHER_VICTORY; EndGamemodeRound();
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
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; EndGamemodeRound();
                    }
                    else if (peanutAlive == false && humanAlive)
                    {
                        ev.Status = ROUND_END_STATUS.CI_VICTORY; EndGamemodeRound();
                    }
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Massacre.enabled)
            {
                ev.SpawnChaos = true;
                ev.PlayerList = new List<Player>();
            }
        }

        public void EndGamemodeRound()
        {
            if (Massacre.enabled)
            {
                plugin.Info("EndgameRound Function");
                Massacre.roundstarted = false;
                plugin.Server.Round.EndRound();
            }
        }

        public void SpawnDboi(Player player)
        {
            player.ChangeRole(Role.CLASSD, false, false, false, true);
            player.Teleport(SpawnLoc);

            foreach (Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.CUP);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Get ready to die!", false);
        }

        public void SpawnNut(Player player)
        {
            player.Teleport(SpawnLoc);
            player.ChangeRole(Role.SCP_173, false, true, true, true);
            plugin.Info("Spawned " + player.Name + " as SCP-173");
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(35, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the D-bois!", false);
        }
    }
}
