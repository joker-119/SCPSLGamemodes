using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using System.Collections.Generic;
using System;
using scp4aiur;

namespace MassacreGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Massacre of the D-Bois Gamemode",
        description = "Gamemode Template",
        id = "gamemode.massacre",
        version = "1.6.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]
    public class Massacre : Plugin
    {
        internal static Massacre singleton;
        
        public static bool
            enabled = false,
            roundstarted = false;
        public static string SpawnRoom;
        public static Vector SpawnLoc;
        public static int 
            nut_health,
            nut_count;
        public static Random generator = new System.Random();
        public static List<Vector> SpawnLocs = new List<Vector>();
        
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
            this.AddCommands(new string[] { "massacre", "motdb", "mascr" }, new MassacreCommand());
            this.AddConfig(new ConfigSetting("mass_spawn_room", "jail", false, SettingType.STRING, true, "Where everyone should spawn."));
            this.AddConfig(new ConfigSetting("mass_peanut_health", 1, SettingType.NUMERIC, true, "How much health Peanuts spawn with."));
            this.AddConfig(new ConfigSetting("mass_peanut_count", 3, SettingType.NUMERIC, true, "The number of peanuts selected."));
            Timing.Init(this);
			new Functions(this);
        }
    }
}