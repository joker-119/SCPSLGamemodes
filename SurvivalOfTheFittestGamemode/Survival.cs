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
		id = "survival.Gamemode",
		version = "1.7.0",
		SmodMajor = 3,
		SmodMinor = 3,
		SmodRevision = 0
	)]
	public class Survival : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new System.Random();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; } = false;
		public bool RoundStarted { get; internal set; } = false;

		public float NutDelay { get; private set; }

		public int NutHealth { get; private set; }
		public int NutLimit { get; private set; }

		public string Zone { get; internal set; }

		public List<Room> BlackoutRooms = new List<Room>();

		public override void OnDisable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been disabled.");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			this.AddConfig(new ConfigSetting("survival_peanut_delay", 120f, SettingType.FLOAT, true, "The amount of time to wait before unleading peanuts."));
			this.AddConfig(new ConfigSetting("survival_peanut_health", 173, SettingType.NUMERIC, true, "The amount of health peanuts should have (lower values move faster"));
			this.AddConfig(new ConfigSetting("survival_zone_type", "hcz", false, SettingType.STRING, true, "The zone the event should take place in."));
			this.AddConfig(new ConfigSetting("surv_gamemode_ranks", new string[] { }, SettingType.LIST, true, "The ranks that can use gamemode commands."));

			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

			this.AddCommands(new string[] { "survival", "sotf", "surv" }, new SurvivalCommand(this));

			Timing.Init(this);

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			NutDelay = GetConfigFloat("survival_peanut_delay");
			NutHealth = GetConfigInt("survival_peanut_health");
			Zone = GetConfigString("survival_zone_type");
			ValidRanks = GetConfigList("surv_gamemode_ranks");
		}
	}
}