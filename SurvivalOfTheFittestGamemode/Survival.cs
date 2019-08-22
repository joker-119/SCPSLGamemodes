using System;
using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;

namespace SurvivalGamemode
{
	[PluginDetails(
		author = "Joker119",
		name = "Survival of the Fittest Gamemode",
		description = "Gamemode Template",
		id = "survival.gamemode",
		version = "2.2.0",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 1
	)]
	public class Survival : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new Random();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }

		public float NutDelay { get; private set; }

		public int NutHealth { get; private set; }
		public int NutLimit { get; private set; }

		public string Zone { get; internal set; }

		public List<Room> BlackoutRooms = new List<Room>();

		public override void OnDisable()
		{
			Info(Details.name + " v." + Details.version + " has been disabled.");
		}

		public override void OnEnable()
		{
			Info(Details.name + " v." + Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			AddConfig(new ConfigSetting("survival_peanut_delay", 120f, true, "The amount of time to wait before unleashing peanuts."));
			AddConfig(new ConfigSetting("survival_peanut_health", 173, true, "The amount of health peanuts should have (lower values move faster"));
			AddConfig(new ConfigSetting("survival_peanut_limit", 3, true, "The maximum number of peanuts that can spawn."));
			AddConfig(new ConfigSetting("survival_zone_type", "hcz", false, true, "The zone the event should take place in."));
			AddConfig(new ConfigSetting("surv_gamemode_ranks", new string[] { }, true, "The ranks that can use gamemode commands."));

			AddEventHandlers(new EventsHandler(this));

			AddCommands(new string[] { "survival", "sotf", "surv" }, new SurvivalCommand(this));

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			NutDelay = GetConfigFloat("survival_peanut_delay");
			NutHealth = GetConfigInt("survival_peanut_health");
			Zone = GetConfigString("survival_zone_type");
			ValidRanks = GetConfigList("surv_gamemode_ranks");
			NutLimit = GetConfigInt("survival_peanut_limit");
		}
	}
}