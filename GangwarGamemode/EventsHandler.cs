using System.Linq;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using Smod2.Events;
using System.Collections.Generic;
using scp4aiur;

namespace Gangwar
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerSetRole, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerPlayerJoin, IEventHandlerWaitingForPlayers
    {
        private readonly Gangwar plugin;

        public EventsHandler(Gangwar plugin) => this.plugin = plugin;

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (Gangwar.enabled)
            {
                if (!Gangwar.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=00FFFF> Gangwar Gamemode</color> is starting..", false);
                }
            }
        }
        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (Gangwar.enabled || Gangwar.roundstarted)
            {
                if (ev.Player.TeamRole.Team == Team.SCP || ev.Player.TeamRole.Team == Team.CLASSD)
                {
                    Timing.Run(Functions.singleton.SpawnChaos(ev.Player,0));
                }
                else if (ev.Player.TeamRole.Role == Role.FACILITY_GUARD || ev.Player.TeamRole.Team == Team.SCIENTIST)
                {
                    Timing.Run(Functions.singleton.SpawnNTF(ev.Player, 0));
                }
                else if (ev.Player.TeamRole.Team == Team.SPECTATOR)
                {
                    ev.Player.PersonalBroadcast(25, "You are dead, but don't worry, you will respawn soon!", false);
                }
            }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Gangwar.ci_health = this.plugin.GetConfigInt("gangwar_ci_health");
            Gangwar.ntf_health = this.plugin.GetConfigInt("gangwar_ntf_health");
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Gangwar.enabled)
            {
                Gangwar.roundstarted = true;
                plugin.Server.Map.StartWarhead();
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Gangwar Gamemode started!");
                List<Player> players = ev.Server.GetPlayers();

                for (int i = 0; i < (players.Count / 2); i++)
                {
                    int random = new System.Random().Next(players.Count);
                    Player randomplayer = players[random];
                    players.Remove(randomplayer);
                    Timing.Run(Functions.singleton.SpawnNTF(randomplayer,0));
                }
                foreach (Player player in players)
                {
                    Timing.Run(Functions.singleton.SpawnChaos(player,0));
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Gangwar.enabled ||Gangwar.roundstarted)
            {
                plugin.Info("Round Ended!");
                Functions.singleton.EndGamemodeRound();
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Gangwar.enabled || Gangwar.roundstarted)
            {
                bool ciAlive = false;
                bool ntfAlive = false;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if(player.TeamRole.Team == Team.CHAOS_INSURGENCY)
                    {
                        ciAlive = true; continue;
                    }
                    else if (player.TeamRole.Team == Team.NINETAILFOX)
                    {
                        ntfAlive = true;
                    }
                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (ciAlive && ntfAlive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                        ev.Server.Map.ClearBroadcasts();
                        ev.Server.Map.Broadcast(10, "There are " + plugin.Round.Stats.CiAlive + " Chaos alive, and " + plugin.Round.Stats.NTFAlive + " NTF alive.", false);
                    }
                    else if (ciAlive && ntfAlive == false)
                    {
                        ev.Status = ROUND_END_STATUS.OTHER_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                    else if (ciAlive == false && ntfAlive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Gangwar.enabled || Gangwar.roundstarted)
            {
                if (plugin.Round.Stats.CiAlive >= plugin.Round.Stats.NTFAlive)
                {
                    ev.SpawnChaos = false;
                }
                else if (plugin.Round.Stats.CiAlive < plugin.Round.Stats.NTFAlive)
                {
                    ev.SpawnChaos = true;
                }
            }
        }
    }
}