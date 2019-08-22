using System;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;

namespace ThreeMusketeers
{
	[PluginDetails(
		author = "Joker119",
		name = "Three Musketeers Gamemode",
		description = "3 NTF Vs. a crap load of Class-D",
		id = "musketeers.gamemode",
		version = "2.1.1",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Musketeers : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new Random();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }

		public int NtfHealth { get; private set; }
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

			AddEventHandlers(new EventsHandler(this));

			AddCommands(new string[] { "3musketeers", "musketeers", "3musk" }, new MusketeersCommand(this));

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			NtfHealth = GetConfigInt("musk_ntf_health");
			ClassDHealth = GetConfigInt("musk_classd_health");
			ValidRanks = GetConfigList("musk_gamemode_ranks");
		}
	}
}