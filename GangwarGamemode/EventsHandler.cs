using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using Smod2.Events;

namespace GangwarGamemode
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerSetRole, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerPlayerJoin
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
                if (ev.Player.TeamRole.Team == Smod2.API.Team.SCP)
                {
                    SpawnChaos(ev.Player);
                }
                else if (ev.Player.TeamRole.Team == Smod2.API.Team.NINETAILFOX || ev.Player.TeamRole.Team == Smod2.API.Team.SCIENTIST || ev.Player.TeamRole.Team == Smod2.API.Team.NINETAILFOX)
                {
                    SpawnNTF(ev.Player);
                }
                else if (ev.Player.TeamRole.Team == Smod2.API.Team.SPECTATOR)
                {
                    ev.Player.PersonalBroadcast(25, "You are dead, but don't worry, you will respawn soon!", false);
                }
            }
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Gangwar.enabled)
            {
                Gangwar.ci_health = this.plugin.GetConfigInt("Gangwar_ci_health");
                Gangwar.ntf_health = this.plugin.GetConfigInt("Gangwar_ntf_health");

                PluginManager.Manager.Server.Map.DetonateWarhead();

                Gangwar.roundstarted = true;
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Gangwar Gamemode started!");

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Smod2.API.Team.SCP || player.TeamRole.Team == Smod2.API.Team.CLASSD)
                    {
                        SpawnChaos(player);
                    }
                    else if (player.TeamRole.Team == Smod2.API.Team.NINETAILFOX || player.TeamRole.Team == Smod2.API.Team.SCIENTIST || player.TeamRole.Team == Smod2.API.Team.NINETAILFOX)
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
                    if(player.TeamRole.Team == Smod2.API.Team.CHAOS_INSURGENCY)
                    {
                        ciAlive = true; continue;
                    }
                    else if (player.TeamRole.Team == Smod2.API.Team.NINETAILFOX)
                    {
                        ntfAlive = true;
                    }
                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (ciAlive && ntfAlive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (ciAlive && ntfAlive == false)
                    {
                        ev.Status = ROUND_END_STATUS.CI_VICTORY; EndGamemodeRound();
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
                int ci_count = 0;
                int ntf_count = 0;
                foreach (Player player in plugin.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Smod2.API.Team.CHAOS_INSURGENCY)
                    {
                        ci_count = ci_count + 1;
                    }
                    else if (player.TeamRole.Team == Smod2.API.Team.NINETAILFOX)
                    {
                        ntf_count = ntf_count + 1;
                    }
                }
                if (ci_count >= ntf_count)
                {
                    ev.SpawnChaos = false;
                }
                else if (ci_count < ntf_count)
                {
                    ev.SpawnChaos = true;
                }
            }
        }

        public void EndGamemodeRound()
        {
            plugin.Info("EndgameRound Function.");
            Gangwar.roundstarted = false;
            plugin.Server.Round.EndRound();
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