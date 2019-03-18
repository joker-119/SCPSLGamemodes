using System.Runtime.CompilerServices;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using scp4aiur;
using UnityEngine;

namespace PresidentialEscortGamemode
{
    internal class EventsHandler : IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerJoin, IEventHandlerRoundEnd
    {
        private readonly PresidentialEscort plugin;

        public EventsHandler(PresidentialEscort plugin) => this.plugin = plugin;

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
                foreach(Player player in players)
                {
                    if(player.TeamRole.Team == Smod2.API.Team.SCP)
                    {
                        players.Remove(player);
                    }
                }

                // chooses and spawns VIP scientist
                int chosenVIP = new System.Random().Next(players.Count);
                Player vip = players[chosenVIP];
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

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (PresidentialEscort.enabled || PresidentialEscort.roundstarted)
            {
                bool vipAlive = false;
                bool scpAlive = false;
                bool vipUpgraded = false;
                bool vipEscaped = false;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Smod2.API.Team.SCP)
                    {
                        scpAlive = true; continue;
                    }

                    else if (player == PresidentialEscort.vip)
                    {
                        vipAlive = true;

                        if (player.TeamRole.Team != Smod2.API.Team.SCIENTIST)
                        {
                            vipUpgraded = true;
                            if (player.HasItem(ItemType.FLASHLIGHT)) vipEscaped = true;
                        }
                    }
                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (vipEscaped)
                    {
                        ev.Status = ROUND_END_STATUS.OTHER_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                    else if (vipUpgraded)
                    {
                        ev.Status = ROUND_END_STATUS.NO_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                    else if (vipAlive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (scpAlive && !vipAlive)
                    {
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                    else
                    {
                        ev.Status = ROUND_END_STATUS.NO_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                }
            }
        }
    }
}
