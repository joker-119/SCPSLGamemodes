using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;

namespace LurkingGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Lurking in the dark Gamemode",
        description = "Lurking in the Dark Gamemode",
        id = "gamemode.lurking",
        version = "1.3.5",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
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
        public static bool blackouts;

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
    }
    public class Functions
    {
        public static void EndGamemodeRound()
        {
            if (Lurking.enabled)
            {
                Lurking.plugin.Info("EndgameRound Function");
                Lurking.roundstarted = false;
                Lurking.plugin.Server.Round.EndRound();

                if (Lurking.blackouts)
                {
                    SCP575.Functions.singleton.EnableBlackouts();
                    Lurking.plugin.Info("Enabling timed Blackouts.");
                }
                SCP575.Functions.singleton.ToggleBlackout();
            }
        }

        public static void SpawnLarry(Player player)
        {
            player.ChangeRole(Role.SCP_106, false, true, false, false);
            player.SetHealth(Lurking.larry_health);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
        }
        public static void SpawnDoggo(Player player)
        {
            if (player.TeamRole.Role != Role.SCP_106)
            {
                player.ChangeRole(Role.SCP_939_53, false, true, false, false);
                player.SetHealth(Lurking.doggo_health);
                player.PersonalClearBroadcasts();
                player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
            }
        }
        public static void EnableGamemode()
        {
            Lurking.enabled = true;
            if (!Lurking.roundstarted)
            {
                Lurking.plugin.pluginManager.Server.Map.ClearBroadcasts();
                Lurking.plugin.pluginManager.Server.Map.Broadcast(25, "<color=#2D2B2B> Lurking in the Dark</color> Gamemode starting..", false);
            }
        }

        public static void DisableGamemode()
        {
            Lurking.enabled = false;
            Lurking.plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
    }
}