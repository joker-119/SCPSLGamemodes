using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using Smod2.Events;
using System.Timers;


namespace ZombielandGamemode
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerDoorAccess, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerHurt, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerSetRole, IEventHandlerWaitingForPlayers
    {
        private readonly Zombieland plugin;

        public EventsHandler(Zombieland plugin) => this.plugin = plugin;
        public static Timer timer;

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (Zombieland.enabled)
            {
                if (!Zombieland.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting...", false);
                }
            }
        }
        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (Zombieland.enabled || Zombieland.roundstarted)
            {
               if (ev.TeamRole.Team == Team.SCP)
               {
                   Functions.SpawnZombie(ev.Player);
               }
               else if (ev.TeamRole.Team != Team.SPECTATOR)
               {
                    ev.Player.PersonalBroadcast(25, "You are a human! You must escape the zombie outbreak! Any human deaths from any cause will result in more zombies! When killed, zombies respawn as Chaos! Good Luck!", false);
               }
               else if (ev.TeamRole.Team == Team.SPECTATOR)
               {
                    ev.Player.PersonalBroadcast(25, "You are dead! But don't worry, you'll respawn as Chaos soon to fight the zombies!", false);
               }
            }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Zombieland.zombie_health = this.plugin.GetConfigInt("zombieland_zombie_health");
            Zombieland.child_health = this.plugin.GetConfigInt("zombieland_child_health");
            Zombieland.AlphaDoorDestroy = this.plugin.GetConfigBool("zombieland_alphas_destroy_doors");
        }

        public void OnRoundStart(RoundStartEvent ev)
        {

            if (Zombieland.enabled)
            {
                Zombieland.roundstarted = true;
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Zombieland Gamemode Started!");

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Role == Role.SCP_049_2)
                    {
                        player.SetHealth(Zombieland.zombie_health);
                    }
                }

                timer = new Timer();
                timer.Interval = 90000;
                timer.AutoReset = true;
                timer.Enabled = true;
                timer.Elapsed += OnTimedEvent;
            }
        }

        public void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            if (Zombieland.enabled || Zombieland.roundstarted)
            {
                plugin.Server.Map.ClearBroadcasts();
                int human_count = (Zombieland.plugin.Round.Stats.NTFAlive + Zombieland.plugin.Round.Stats.ScientistsAlive + Zombieland.plugin.Round.Stats.ClassDAlive + Zombieland.plugin.Round.Stats.CiAlive);
                plugin.Server.Map.Broadcast(10, "There are currently " + Zombieland.plugin.Round.Stats.Zombies + " zombies and " + human_count + " humans alive.", false);
            }
        }

        public void OnDoorAccess(PlayerDoorAccessEvent ev)
        {
            if (ev.Door.Locked && Zombieland.Alpha.Contains(ev.Player) && Zombieland.AlphaDoorDestroy)
            {
                ev.Destroy = true;
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Zombieland.enabled || Zombieland.roundstarted)
                plugin.Info("Round Ended!");
                Functions.EndGamemodeRound();
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Zombieland.enabled || Zombieland.roundstarted)
            {
                bool zombieAlive = false;
                bool humanAlive = false;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (player.TeamRole.Team == Team.SCP)
                    {
                        zombieAlive = true; continue;
                    }

                    else if (player.TeamRole.Team != Team.SCP && player.TeamRole.Team != Team.SPECTATOR)
                        humanAlive = true;
                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (zombieAlive && humanAlive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (zombieAlive && humanAlive == false)
                    {
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; Functions.EndGamemodeRound();
                    }
                    else if (zombieAlive == false && humanAlive)
                    {
                        ev.Status = ROUND_END_STATUS.CI_VICTORY; Functions.EndGamemodeRound();
                    }
                }
            }
        }

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if ((Zombieland.enabled || Zombieland.roundstarted) && ev.Player.TeamRole.Team != Team.SCP && ev.Damage > ev.Player.GetHealth())
            {
                if (ev.Attacker == ev.Player || ev.DamageType == DamageType.TESLA || ev.DamageType == DamageType.NUKE || ev.DamageType == DamageType.LURE)
                {
                    ev.Player.ChangeRole(Role.SPECTATOR);
                }
                else
                {
                    ev.Damage = 0;
                    Functions.SpawnChild(ev.Player);
                }
            }   
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Zombieland.enabled || Zombieland.roundstarted)
            {
                ev.SpawnChaos = true;
            }
        }
    }
}
