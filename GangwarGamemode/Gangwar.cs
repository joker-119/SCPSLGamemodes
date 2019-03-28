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
        id = "gangwar.Gamemode",
        version = "1.7.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class Gangwar : Plugin
    {
        public Functions Functions { get; private set; }
        public Random Gen = new System.Random();

        public string[] ValidRanks;

        public bool Enabled { get; internal set; }
        public bool RoundStarted { get; internal set; }

        public int CIHealth { get; private set; }
        public int NTFHealth { get; private set; }


        public override void OnDisable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " has been diisabled.");
        }

        public override void OnEnable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " has been Enabled.");
        }

        public override void Register()
        {
            this.AddConfig(new ConfigSetting("gangwar_ci_health", 120, SettingType.NUMERIC, true, "The amount of health CI have."));
            this.AddConfig(new ConfigSetting("gangwar_ntf_health", 150, SettingType.NUMERIC, true, "The amount of health NTF have."));
            this.AddConfig(new ConfigSetting("gamemode_ranks", new string[] { "owner", "admin" }, SettingType.LIST, true, "The ranks able to use commands."));

            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "gangwar", "gang", "gw" }, new GangwarCommand(this));

            Timing.Init(this);

            Functions = new Functions(this);
        }

        public void ReloadConfig()
        {
            CIHealth = GetConfigInt("gangwar_ci_health");
            NTFHealth = GetConfigInt("gangwar_ntf_health");
            ValidRanks = GetConfigList("gamemode_ranks");
        }
    }
}