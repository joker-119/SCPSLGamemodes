using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using System;
using scp4aiur;

namespace MuskateersGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Three Muskateers Gamemode",
        description = "3 NTF Vs. a crap load of Class-D",
        id = "muskateers.gamemode",
        version = "1.5.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class Muskateers : Plugin
    {
        internal static Muskateers singleton;
        public static bool
            enabled = false,
            roundstarted = false;
        public static int ntf_health;
        public static int classd_health;
        public static Random generator = new System.Random();
        
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
			new Functions(this);
			Timing.Init(this);
            this.AddCommands(new string[] { "3muskateers", "muskateers", "3musk" }, new MuskateersCommand());
            this.AddConfig(new ConfigSetting("musk_ntf_health", 4500, SettingType.NUMERIC, true, "How much Health NTF spawn with."));
            this.AddConfig(new ConfigSetting("musk_classd_health", 100, SettingType.NUMERIC, true, "How much health Class-D spawn with."));
        }
    }
}