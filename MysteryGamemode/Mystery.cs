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
        id = "mystery.Gamemode",
        version = "1.7.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class Mystery : Plugin
    {
        public Functions Functions { get; private set; }

        public Random gen = new System.Random();

        public Dictionary<string, bool> murd = new Dictionary<string, bool>();

        public string[] ValidRanks { get; private set; }

        public bool Enabled { get; internal set; } = false;
        public bool RoundStarted { get; internal set; } = false;
        public bool MurdRespawn { get; private set; }
        public bool DetRespawn { get; private set; }

        public int MurdererNum { get; private set; }
        public int DetectiveNum { get; private set; }
        public int DetHealth { get; private set; }
        public int CivHealth { get; private set; }
        public int MurdHealth { get; private set; }
        public int MonserNum { get; private set; }

        public override void OnDisable()
        {
            this.Info(this.Details.name + "v." + this.Details.version + " has been disbaled.");
        }

        public override void OnEnable()
        {
            this.Info(this.Details.name + "v." + this.Details.version + " has been Enabled.");
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
            this.AddConfig(new ConfigSetting("gamemode_ranks", new string[] { "owner", "admin" }, SettingType.LIST, true, "The ranks able to use commands."));

            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

            this.AddCommands(new string[] { "mystery", "murder" }, new MysteryCommand(this));

            Timing.Init(this);

            Functions = new Functions(this);
        }

        public void ReloadConfig()
        {
            CivHealth = GetConfigInt("myst_civ_health");
            DetHealth = GetConfigInt("myst_det_health");
            MurdHealth = GetConfigInt("myst_murd_health");
            DetectiveNum = GetConfigInt("myst_det_num");
            MonserNum = GetConfigInt("myst_monster_num");
            MurdererNum = GetConfigInt("myst_murd_num");
            DetRespawn = GetConfigBool("myst_det_respawn");
            MurdRespawn = GetConfigBool("myst_murd_respawn");
            ValidRanks = GetConfigList("gamemode_ranks");
        }
    }
}