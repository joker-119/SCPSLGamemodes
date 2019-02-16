using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using Smod2.Events;
using System.Timers;


namespace ZombielandGamemode
{
    internal class EventsHandler : IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerHurt, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerSetRole, IEventHandlerWaitingForPlayers
    {
        private readonly Zombieland plugin;

        public EventsHandler(Zombieland plugin) => this.plugin = plugin;

        private int zombie_health;
        private int child_health;
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
            if (Zombieland.enabled)
            {
               if (ev.TeamRole.Team == Team.SCP && ev.TeamRole.Role != Role.SCP_049_2)
               {
                   SpawnZombie(ev.Player);
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
                    if (player.TeamRole.Team == Team.SCP)
                    {
                        SpawnZombie(player);
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
            if (Zombieland.enabled)
            {
                plugin.Server.Map.ClearBroadcasts();
                int human_count = (Zombieland.plugin.Round.Stats.NTFAlive + Zombieland.plugin.Round.Stats.ScientistsAlive + Zombieland.plugin.Round.Stats.ClassDAlive + Zombieland.plugin.Round.Stats.CiAlive) - 1;
                plugin.Server.Map.Broadcast(10, "There are currently " + Zombieland.plugin.Round.Stats.Zombies + " zombies and " + human_count + " humans alive.", false);
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Zombieland.enabled)
                plugin.Info("Round Ended!");
                EndGamemodeRound();
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Zombieland.enabled)
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
                        ev.Status = ROUND_END_STATUS.SCP_VICTORY; EndGamemodeRound();
                    }
                    else if (zombieAlive == false && humanAlive)
                    {
                        ev.Status = ROUND_END_STATUS.CI_VICTORY; EndGamemodeRound();
                    }
                }
            }
        }

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (Zombieland.enabled && ev.Player.TeamRole.Team != Team.SCP && ev.Damage > ev.Player.GetHealth())
            {
                if (ev.Attacker == ev.Player || ev.DamageType == DamageType.TESLA)
                {
                    ev.Player.ChangeRole(Role.SPECTATOR);
                }
                else
                {
                    ev.Damage = 0;
                    SpawnChild(ev.Player);
                }
            }   
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Zombieland.enabled)
            {
                ev.SpawnChaos = true;
            }
        }

        public void EndGamemodeRound()
        {
            if (Zombieland.enabled)
            {
                plugin.Info("EndgameRound Function");
                Zombieland.roundstarted = false;
                plugin.Server.Round.EndRound();
            }

        }

        public void SpawnChild(Player player)
        {
            player.ChangeRole(Role.SCP_049_2, false, false, false, true);


            child_health = Zombieland.child_health;
            player.SetHealth(child_health);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You died and became a <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
        }

        public void SpawnZombie(Player player)
        {
            Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);
            player.ChangeRole(Role.SCP_049_2, false, false, true, true);
            player.Teleport(spawn);

            zombie_health = Zombieland.zombie_health;
            player.SetHealth(zombie_health);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are an alpha <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
        }
    }
}
