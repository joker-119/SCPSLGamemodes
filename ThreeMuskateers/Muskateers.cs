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
		id = "muskateers.Gamemode",
		version = "1.8.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Muskateers : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new System.Random();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; } = false;
		public bool RoundStarted { get; internal set; } = false;

		public int NTFHealth { get; private set; }
		public int ClassDHealth { get; private set; }

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
			AddConfig(new ConfigSetting("musk_ntf_health", 4500, true, "How much Health NTF spawn with."));
			AddConfig(new ConfigSetting("musk_classd_health", 100, true, "How much health Class-D spawn with."));
			AddConfig(new ConfigSetting("musk_gamemode_ranks", new string[] { }, true, "The ranks able to use gamemode commands."));

			AddEventHandlers(new EventsHandler(this), Priority.Normal);

			AddCommands(new string[] { "3muskateers", "muskateers", "3musk" }, new MuskateersCommand(this));

			Timing.Init(this);

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			NTFHealth = GetConfigInt("musk_ntf_health");
			ClassDHealth = GetConfigInt("musk_classd_health");
			ValidRanks = GetConfigList("musk_gamemode_ranks");
		}
	}
}