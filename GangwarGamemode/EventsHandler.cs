using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using Smod2.Events;

namespace GangwarGamemode
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
            if (Gangwar.enabled)
            {
                if (ev.Player.TeamRole.Team == Team.SCP || ev.Player.TeamRole.Team == Team.CLASSD)
                {
                    SpawnChaos(ev.Player);
                }
                else if (ev.Player.TeamRole.Role == Role.FACILITY_GUARD || ev.Player.TeamRole.Team == Team.SCIENTIST)
                {
                    SpawnNTF(ev.Player);
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
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Gangwar Gamemode started!");

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Team.SCP || player.TeamRole.Team == Team.CLASSD)
                    {
                        SpawnChaos(player);
                    }
                    else if (player.TeamRole.Role == Role.FACILITY_GUARD || player.TeamRole.Team == Team.SCIENTIST)
                    {
                        SpawnNTF(player);
                    }
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Gangwar.enabled)
            {
                plugin.Info("Round Ended!");
                EndGamemodeRound();
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Gangwar.enabled)
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
                        ev.Status = ROUND_END_STATUS.OTHER_VICTORY; EndGamemodeRound();
                    }
                    else if (ciAlive == false && ntfAlive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY; EndGamemodeRound();
                    }
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Gangwar.enabled)
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

        public void EndGamemodeRound()
        {
            if (Gangwar.enabled)
            {
                plugin.Info("EndgameRound Function.");
                Gangwar.roundstarted = false;
                plugin.Server.Round.EndRound();
            }
        }

        public void SpawnChaos(Player player)
        {
            player.ChangeRole(Role.CHAOS_INSURGENCY, true, true, false, true);
            player.SetHealth(Gangwar.ci_health);
        }

        public void SpawnNTF(Player player)
        {
            player.ChangeRole(Role.NTF_COMMANDER, true, true, false, true);
            player.SetHealth(Gangwar.ntf_health);
        }
    }
}