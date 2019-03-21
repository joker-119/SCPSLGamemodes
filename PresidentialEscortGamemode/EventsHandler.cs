using System.Runtime.CompilerServices;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using scp4aiur;
using UnityEngine;
using System.Linq;

namespace PresidentialEscortGamemode
{
    internal class EventsHandler : IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerCheckEscape, IEventHandlerWaitingForPlayers
    {
        private readonly PresidentialEscort plugin;

        public EventsHandler(PresidentialEscort plugin) => this.plugin = plugin;

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            PresidentialEscort.vip_health = this.plugin.GetConfigInt("vip_vip_health");
            PresidentialEscort.guard_health = this.plugin.GetConfigInt("vip_guard_health");
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (PresidentialEscort.enabled)
            {
                if (!PresidentialEscort.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=#f8ea56>Presidential Escort</color> gamemode is starting...", false);
                }
            }
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (PresidentialEscort.enabled)
            {
                PresidentialEscort.roundstarted = true;
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Presidential Escort Gamemode Started!");
                List<Player> players = ev.Server.GetPlayers();

                // removes SCPs from player list (used to spawn rest of players)
                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Smod2.API.Team.SCP)
                    {
                        players.Remove(player);
                    }
                }

                // chooses and spawns VIP scientist
                Player vip;
                if (!(PresidentialEscort.vip is Player))
                {
                    int chosenVIP = new System.Random().Next(players.Count);
                    vip = players[chosenVIP];
                }
                else
                    vip = PresidentialEscort.vip;

                plugin.Info("" + vip.Name + " chosen as the VIP");
                Timing.Run(Functions.singleton.SpawnVIP(vip));
                players.Remove(vip);

                // spawn NTF into round
                foreach (Player player in players)
                {
                    Timing.Run(Functions.singleton.SpawnNTF(player));
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (PresidentialEscort.enabled || PresidentialEscort.roundstarted)
                plugin.Info("Round Ended!");
            Functions.singleton.EndGamemodeRound();
        }
        public void OnCheckEscape(PlayerCheckEscapeEvent ev)
        {
            if (ev.Player.SteamId == PresidentialEscort.vip.SteamId)
                PresidentialEscort.vipEscaped = true;
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (PresidentialEscort.enabled || PresidentialEscort.roundstarted)
            {
                bool vipAlive = false;
                bool scpAlive = false;
                bool vipEscaped = false;

                if (!(PresidentialEscort.vip is Player))
                {
                    plugin.Info("VIP not found. Ending gamemode.");
                    ev.Status = ROUND_END_STATUS.NO_VICTORY;
                    Functions.singleton.EndGamemodeRound();
                    return;
                }

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Smod2.API.Team.SCP)
                    {
                        scpAlive = true; continue;
                    }
                    else if (player.SteamId == PresidentialEscort.vip.SteamId)
                    {
                        vipAlive = true;
                    }
                }

                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (vipEscaped || (vipAlive && !scpAlive))
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                    else if (vipAlive && scpAlive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (scpAlive && !vipAlive)
                    {
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                }
            }
        }
        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (PresidentialEscort.enabled || PresidentialEscort.roundstarted)
            {
                ev.SpawnChaos = false;
            }
        }
    }
}
