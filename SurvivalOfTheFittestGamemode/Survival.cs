using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using System.Collections.Generic;
using System.Linq;
using System;
using scp4aiur;

namespace SurvivalGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Survival of the Fittest Gamemode",
        description = "Gamemode Template",
        id = "gamemode.survival",
        version = "1.5.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]
    public class Survival : Plugin
    {
        internal static Survival singleton;
		public static Random gen = new System.Random();
        public static bool
            enabled = false,
			blackouts,
            roundstarted = false;
        public static float nut_delay;
        public static int nut_health;
		public static string zone;
        
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
            this.AddCommands(new string[] { "survival", "sotf", "surv" }, new SurvivalCommand());
            Timing.Init(this);
			new Functions(this);
            this.AddConfig(new ConfigSetting("survival_peanut_delay", 120f, SettingType.FLOAT, true, "The amount of time to wait before unleading peanuts."));
            this.AddConfig(new ConfigSetting("survival_peanut_health", 173, SettingType.NUMERIC, true, "The amount of health peanuts should have (lower values move faster"));
			this.AddConfig(new ConfigSetting("survival_zone_type", "hcz", false, SettingType.STRING, true, "The zone the event should take place in."));
        }
    }
}