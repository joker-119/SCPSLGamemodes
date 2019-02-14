using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;

namespace LurkingGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Lurking in the dark Gamemode",
        description = "Lurking in the Dark Gamemode",
        id = "gamemode.lurking",
        version = "1.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2
    )]

    public class Lurking : Plugin
    {
        internal static Lurking plugin;

        public static bool enabled = false;
        public static bool roundstarted = false;
        public static int larry_health;
        public static int doggo_health;
        public static int larry_count;
        public static int doggo_count;

        public override void OnDisable()
        {
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been disbaled.");
        }

        public override void OnEnable()
        {
            plugin = this;
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "lurking", "lurk", "litd" }, new LurkingCommand());

            this.AddConfig(new ConfigSetting("lurking_106_num", 2, SettingType.NUMERIC, true, "The number of Larries to spawn"));
            this.AddConfig(new ConfigSetting("lurking_939_num", 2, SettingType.NUMERIC, true, "The number of 939's to spawn."));
            this.AddConfig(new ConfigSetting("lurking_106_health", 750, SettingType.NUMERIC, true, "The amount of health Larry should start with."));
            this.AddConfig(new ConfigSetting("lurking_939_health", 2300, SettingType.NUMERIC, true, "The amount of health Doggo should start with."));
        }

        public static void EnableGamemode()
        {
            enabled = true;
            if (!roundstarted)
            {
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.pluginManager.Server.Map.Broadcast(25, "<color=#2D2B2B> Lurking in the Dark</color> Gamemode starting..", false);
            }
        }

        public static void DisableGamemode()
        {
            enabled = false;
            plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
    }
}