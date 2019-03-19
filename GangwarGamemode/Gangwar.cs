using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using scp4aiur;
using System;

namespace Gangwar
{
    [PluginDetails(
        author = "Joker119",
        name = "Gangwar Gamemode",
        description = "Gangwar Gamemode",
        id = "gamemode.gangwar",
        version = "1.6.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class Gangwar : Plugin
    {
        internal static Gangwar singleton;
        public static bool 
            enabled = false,
            roundstarted = false;

        public static int ci_health;
        public static int ntf_health;
		public static Random gen = new System.Random();

        public override void OnDisable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " has been diisabled.");
        }

        public override void OnEnable()
        {
            singleton = this;
            this.Info(this.Details.name + " v." + this.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "gangwar", "gang", "gw" }, new GangwarCommand());
            this.AddConfig(new ConfigSetting("gangwar_ci_health", 120, SettingType.NUMERIC, true, "The amount of health CI have."));
            this.AddConfig(new ConfigSetting("gangwar_ntf_health", 150, SettingType.NUMERIC, true, "The amount of health NTF have."));
            Timing.Init(this);
			new Functions(this);
        }
    }
}