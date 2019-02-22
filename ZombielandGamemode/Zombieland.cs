using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;

namespace ZombielandGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Zombieland Gamemode",
        description = "Gamemode Template",
        id = "gamemode.zombieland",
        version = "1.3.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]
    public class Zombieland : Plugin
    {
        internal static Zombieland plugin;
        public static int zombie_health;
        public static int child_health;
        
        public static bool
            enabled = false,
            roundstarted = false;
        
        public override void OnDisable()
        {
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been disabled.");
        }

        public override void OnEnable()
        {
            plugin = this;
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "zombie", "zombieland", "zl" }, new ZombielandCommand());

            this.AddConfig(new ConfigSetting("zombieland_zombie_health", 3000, SettingType.NUMERIC, true, "The amount of health the starting zombies have."));
            this.AddConfig(new ConfigSetting("zombieland_child_health", 500, SettingType.NUMERIC, true, "The amoutn of health child zombies should have."));
        }
    }

    public class Functions
    {
        public static void EnableGamemode()
        {
            Zombieland.enabled = true;
            if (!Zombieland.roundstarted)
            {
                Zombieland.plugin.pluginManager.Server.Map.ClearBroadcasts();
                Zombieland.plugin.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting..", false);
            }
        }
        public static void DisableGamemode()
        {
            Zombieland.enabled = false;
            Zombieland.plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
        public static void EndGamemodeRound()
        {
            if (Zombieland.enabled)
            {
                Zombieland.plugin.Info("EndgameRound Function");
                Zombieland.roundstarted = false;
                Zombieland.plugin.Server.Round.EndRound();
            }

        }

        public static void SpawnChild(Player player)
        {
            player.ChangeRole(Role.SCP_049_2, false, false, false, true);
            player.SetHealth(Zombieland.child_health);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You died and became a <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
        }

        public static void SpawnZombie(Player player)
        {
            Vector spawn = Zombieland.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);
            player.ChangeRole(Role.SCP_049_2, false, false, true, true);
            player.Teleport(spawn);

            player.SetHealth(Zombieland.zombie_health);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are an alpha <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
        }
    }
}