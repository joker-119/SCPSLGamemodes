using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using UnityEngine;
using System.Collections.Generic;
using scp4aiur;
using System;

namespace JuggernautGamemode
{
    [PluginDetails(
        author = "Mozeman",
        name = "Juggernaut Gamemode",
        description = "Gamemode Template",
        id = "gamemode.juggernaut",
        version = "1.5.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
        )]
    public class Juggernaut : Plugin
    {
        internal static Juggernaut singleton;
        public static bool
            NTF_Disarmer,
            jugg_infinite_nades;
        public static int
            Jugg_base,
            Jugg_increase,
            Jugg_grenade,
            NTF_ammo,
            NTF_Health,
            juggernaut_health,
            ntf_health;
        public static Player
            juggernaut = null,
            activator = null,
            selectedJuggernaut = null,
            jugg_killer = null;
        public static float critical_damage;
        public static string[] juggernaut_prevRank = new string[2];
        public static HealthBar health_bar_type;
		public static System.Random gen = new System.Random();



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
            // Register Events
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "jug", "jugg", "juggernaut" }, new JuggernautCommand());
			new Functions(this);
			Timing.Init(this);


            // Register Configs
            this.AddConfig(new ConfigSetting("juggernaut_base_health", 500, SettingType.NUMERIC, true, "The amoutn of base health the Juggernaut starts with."));
            this.AddConfig(new ConfigSetting("juggernaut_increase_amount", 500, SettingType.NUMERIC, true, "The amount of extra HP a Jugg gets for each additional player."));
            this.AddConfig(new ConfigSetting("juggernaut_jugg_grenades", 6, SettingType.NUMERIC, true, "The number of grenades the Jugg should start with."));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_disarmer", false, SettingType.BOOL, true, "Wether or not NTF should spawn with Disarmers."));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_ammo", 272, SettingType.NUMERIC, true, "The amount of ammo NTF Commanders should spawn with."));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_health", 150, SettingType.NUMERIC, true, "The amount of health the first wave of NTF should have."));
            this.AddConfig(new ConfigSetting("juggernaut_critical_damage", (float)0.15, SettingType.FLOAT, true, "The amount of critical damage the Juggernaut should recieve."));
            this.AddConfig(new ConfigSetting("juggernaut_infinite_jugg_nades", false, SettingType.BOOL, true, "If the Juggernaut should have infinite grenades."));
            this.AddConfig(new ConfigSetting("juggernaut_health_bar_type", "bar", false, SettingType.STRING, true, "Type of Health Bar to use for Juggernaut"));
        }
    }

    public enum HealthBar
    {
        Raw,
        Percentage,
        Bar
    }
}