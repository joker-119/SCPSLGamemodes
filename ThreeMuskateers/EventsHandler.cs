using System.Linq;
using System;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using scp4aiur;


namespace MuskateersGamemode
{
    internal class EventsHandler : IEventHandlerWaitingForPlayers, IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerTeamRespawn, IEventHandlerPlayerJoin, IEventHandlerPlayerDie
    {
        private readonly Muskateers plugin;
        public EventsHandler(Muskateers plugin) => this.plugin = plugin;

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (Muskateers.enabled)
            {
                if (!Muskateers.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=#308ADA>Three Muskateers</color> gamemode is starting..", false);
                }
            }
        }
        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Muskateers.ntf_health = this.plugin.GetConfigInt("musk_ntf_health");
            Muskateers.classd_health = this.plugin.GetConfigInt("musk_classd_health");
        }
        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Muskateers.enabled)
            {
                Muskateers.roundstarted = true;
                List<Player> players = ev.Server.GetPlayers();
                List<Player> muskateer = new List<Player>();
                List<Player> classd = new List<Player>();

                if (players.Count > 4)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int random = Muskateers.generator.Next(players.Count);
                        Player randomPlayer = players[random];
                        players.Remove(randomPlayer);
                        Timing.Run(Functions.singleton.SpawnNTF(randomPlayer));
                    }
                    foreach (Player player in players)
                    {
                        Timing.Run(Functions.singleton.SpawnClassD(player));
                    }
                }
                else
                {
                    plugin.Error("You must have at least 4 players to play this gamemode.");
                    Muskateers.enabled = false;
                }
            }
        }
        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Muskateers.enabled || Muskateers.roundstarted)
            {
                ev.SpawnChaos = true;
                ev.PlayerList = new List<Player>();
            }
        }
        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Muskateers.enabled || Muskateers.roundstarted)
            {
                plugin.Info("Round Ended!");
                Functions.singleton.EndGamemodeRound();
            }
        }
        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Muskateers.enabled || Muskateers.roundstarted)
            {
                bool muskyAlive = false;
                bool classDAlive = false;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Team.CLASSD || player.TeamRole.Team == Team.CHAOS_INSURGENCY)
                    {
                        classDAlive = true; continue;
                    }
                    else if (player.TeamRole.Team == Team.NINETAILFOX)
                    {
                        muskyAlive = true;
                    }
                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (classDAlive && muskyAlive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (classDAlive && !muskyAlive)
                    {
                        ev.Status = ROUND_END_STATUS.OTHER_VICTORY;
                        Functions.singleton.EndGamemodeRound();
                    }
                    else if (!classDAlive && muskyAlive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY;
                        Functions.singleton.EndGamemodeRound();
                    }
                }
            }
        }
        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Muskateers.enabled || Muskateers.roundstarted)
            {
                plugin.Server.Map.ClearBroadcasts();
                if (ev.Player.TeamRole.Team == Team.NINETAILFOX)
                {
                    plugin.Server.Map.Broadcast(15, "There are now " + (plugin.Server.Round.Stats.NTFAlive - 1) + "<color=#308ADA> Muskateers alive!</color>", false);
                }
                else if (ev.Player.TeamRole.Team != Team.NINETAILFOX)
                {
                    plugin.Server.Map.Broadcast(15, "There are now " + ((plugin.Server.Round.Stats.ClassDAlive + plugin.Server.Round.Stats.CiAlive) - 1) + "<color=#DAA130> Class-D alive!</color>", false);
                }
            }
        }
    }
}