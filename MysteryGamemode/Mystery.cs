using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Events;
using System;
using System.Collections.Generic;
using scp4aiur;

namespace Mystery
{
    [PluginDetails(
        author = "Joker119",
        name = "Mystery Gamemode",
        description = "Murder Mystery Gamemode",
        id = "Mystery.Gamemode",
        version = "1.6.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class Mystery : Plugin
    {
        internal static Mystery singleton;
        public static Random gen = new System.Random();
        public static Dictionary<string, bool> murd = new Dictionary<string, bool>();
        public static bool
            enabled = false,
            roundstarted = false,
            murd_respawn,
            det_respawn;
        public static int
            murderer_num,
            detective_num,
            murder_health,
            det_health,
            civ_health,
            monster_num;

        public override void OnDisable()
        {
            this.Info(this.Details.name + "v." + this.Details.version + " has been disbaled.");
        }
        public override void OnEnable()
        {
            singleton = this;
            this.Info(this.Details.name + "v." + this.Details.version + " has been enabled.");
        }
        public override void Register()
        {
            this.AddConfig(new ConfigSetting("myst_murd_health", 150, SettingType.NUMERIC, true, "How much health murderers should have."));
            this.AddConfig(new ConfigSetting("myst_civ_health", 100, SettingType.NUMERIC, true, "The amount of health civilians have."));
            this.AddConfig(new ConfigSetting("myst_det_health", 150, SettingType.NUMERIC, true, "How much health detectives should have."));
            this.AddConfig(new ConfigSetting("myst_murd_num", 3, SettingType.NUMERIC, true, "The number of murderers to have."));
            this.AddConfig(new ConfigSetting("myst_det_num", 2, SettingType.NUMERIC, true, "The number of detectives to have."));
            this.AddConfig(new ConfigSetting("myst_monster_num", 3, SettingType.NUMERIC, true, "The number of monsters that should be in the game."));
            this.AddConfig(new ConfigSetting("myst_murd_respawn", true, SettingType.BOOL, true, "If a random murderer should be respawned."));
            this.AddConfig(new ConfigSetting("myst_det_respawn", true, SettingType.BOOL, true, "If a random Detective should be respawned."));
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "mystery", "murder" }, new MysteryCommand());
            new Functions(this);
            Timing.Init(this);
        }
    }
}