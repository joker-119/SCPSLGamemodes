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
		version = "1.7.0",
		SmodMajor = 3,
		SmodMinor = 3,
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
			this.Info(this.Details.name + " v." + this.Details.version + " has been disabled.");
		}
		public override void OnEnable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been Enabled.");
		}
		public override void Register()
		{
			this.AddConfig(new ConfigSetting("musk_ntf_health", 4500, SettingType.NUMERIC, true, "How much Health NTF spawn with."));
			this.AddConfig(new ConfigSetting("musk_classd_health", 100, SettingType.NUMERIC, true, "How much health Class-D spawn with."));
			this.AddConfig(new ConfigSetting("musk_gamemode_ranks", new string[] { }, SettingType.LIST, true, "The ranks able to use gamemode commands."));

			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

			this.AddCommands(new string[] { "3muskateers", "muskateers", "3musk" }, new MuskateersCommand(this));

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