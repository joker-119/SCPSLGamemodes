using System.Linq;
using System;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;


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
                    server.Map.Broadcast(25, "<color=#308ADA>Three Muskateers<color> gamemode is starting..", false);
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
                List<Player> players = ev.Server.GetPlayers();
                List<Player> muskateer = new List<Player>();

                for (int i = 0; i < 3; i++)
                {
                    int randomPlayer = new System.Random().Next(players.Count);
                        muskateer.Add(players[randomPlayer]);
                }
                foreach (Player player in players)
                {
                    if (muskateer.Contains(player))
                    {
                        Functions.SpawnNTF(player);
                    }
                    else
                    {
                        Functions.SpawnClassD(player);
                    }
                }
            }
        }
        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Muskateers.enabled)
            {
                ev.SpawnChaos = true;
            }
        }
        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Muskateers.enabled)
            {
                plugin.Info("Round Ended!");
                Functions.EndGamemodeRound();
            }
        }
        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Muskateers.enabled)
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
                        Functions.EndGamemodeRound();
                    }
                    else if (!classDAlive && muskyAlive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY;
                        Functions.EndGamemodeRound();
                    }
                }
            }
        }
        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Muskateers.enabled)
            {
                plugin.Server.Map.ClearBroadcasts();
                if (ev.Player.TeamRole.Team == Team.NINETAILFOX)
                {
                    plugin.Server.Map.Broadcast(15, "There are now " + (Muskateers.plugin.pluginManager.Server.Round.Stats.NTFAlive -1) + "<color=#308ADA> Muskateers alive!</color>", false);
                }
                else if (ev.Player.TeamRole.Team != Team.NINETAILFOX)
                {
                    plugin.Server.Map.Broadcast(15, "There are now " + ((plugin.Server.Round.Stats.ClassDAlive + plugin.Server.Round.Stats.CiAlive) -1) + "<color=#DAA130> Class-D alive!</color>", false);
                }
            }
        }
    }
}