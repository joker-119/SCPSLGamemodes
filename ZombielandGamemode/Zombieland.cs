using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using System.Collections.Generic;
using scp4aiur;

namespace ZombielandGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Zombieland Gamemode",
        description = "Gamemode Template",
        id = "gamemode.zombieland",
        version = "1.3.9",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]
    public class Zombieland : Plugin
    {
        internal static Zombieland singleton;
        public static int zombie_health;
        public static int child_health;
        public static List<Player> Alpha = new List<Player>();
        public static bool AlphaDoorDestroy;
        
        public static bool
            enabled = false,
            roundstarted = false;
        
        public override void OnDisable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " has been disabled.");
        }

        public override void OnEnable()
        {
            singleton = this;
            this.Info(this.Details.name + " v." + this.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "zombie", "zombieland", "zl" }, new ZombielandCommand());
            Timing.Init(this);
			new Functions(this);
            this.AddConfig(new ConfigSetting("zombieland_zombie_health", 3000, SettingType.NUMERIC, true, "The amount of health the starting zombies have."));
            this.AddConfig(new ConfigSetting("zombieland_child_health", 500, SettingType.NUMERIC, true, "The amoutn of health child zombies should have."));
            this.AddConfig(new ConfigSetting("zombieland_alphas_destroy_doors", true, SettingType.BOOL, true, "If Alpha zombies should destroy locked doors."));
        }
    }
}