using System.Collections.Generic;
using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using MEC;
using System;

namespace Gangwar
{
	[PluginDetails(
		author = "Joker119",
		name = "Gangwar Gamemode",
		description = "Gangwar Gamemode",
		id = "gangwar.gamemode",
		version = "2.0.0-gmm",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Gangwar : Plugin
	{
		public Functions Functions { get; private set; }
		public Random Gen = new System.Random();

		public Dictionary<string, bool> Spawning = new Dictionary<string, bool>();

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
			Info(Details.name + " v." + Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			AddConfig(new ConfigSetting("gangwar_ci_health", 120, true, "The amount of health CI have."));
			AddConfig(new ConfigSetting("gangwar_ntf_health", 150, true, "The amount of health NTF have."));
			AddConfig(new ConfigSetting("gang_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this), Priority.Normal);
			AddCommands(new string[] { "gangwar", "gang", "gw" }, new GangwarCommand(this));

			GamemodeManager.GamemodeManager.RegisterMode(this, "-1");

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			CIHealth = GetConfigInt("gangwar_ci_health");
			NTFHealth = GetConfigInt("gangwar_ntf_health");
			ValidRanks = GetConfigList("gang_gamemode_ranks");
		}
	}
}