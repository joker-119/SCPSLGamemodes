using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;

namespace ZombielandGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Zombieland Gamemode",
        description = "Gamemode Template",
        id = "gamemode.zombieland",
        version = "1.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2
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

            this.AddConfig(new ConfigSetting("Zombieland_zombie_health", 3000, SettingType.NUMERIC, true, "The amount of health the starting zombies have."));
            this.AddConfig(new ConfigSetting("Zombieland_child_health", 500, SettingType.NUMERIC, true, "The amoutn of health child zombies should have."));
        }

        public static void EnableGamemode()
        {
            enabled = true;
            if (!roundstarted)
            {
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting..", false);
            }
        }
        public static void DisableGamemode()
        {
            enabled = false;
            plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
    }
}