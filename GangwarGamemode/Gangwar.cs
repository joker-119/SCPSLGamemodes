using System;
using System.Collections.Generic;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;

namespace Gangwar
{
	[PluginDetails(
		author = "Joker119",
		name = "Gangwar Gamemode",
		description = "Gangwar Gamemode",
		id = "gangwar.gamemode",
		version = "2.0.1-gmm",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Gangwar : Plugin
	{
		public Functions Functions { get; private set; }
		public Random Gen = new Random();

		public Dictionary<string, bool> Spawning = new Dictionary<string, bool>();

		public bool RoundStarted { get; internal set; }

		public int CiHealth { get; private set; }
		public int NtfHealth { get; private set; }


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
			AddConfig(new ConfigSetting("gangwar_ci_health", 120, true, "The amount of health CI have."));
			AddConfig(new ConfigSetting("gangwar_ntf_health", 150, true, "The amount of health NTF have."));
			AddConfig(new ConfigSetting("gang_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this));

			GamemodeManager.GamemodeManager.RegisterMode(this);

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			CiHealth = GetConfigInt("gangwar_ci_health");
			NtfHealth = GetConfigInt("gangwar_ntf_health");
		}
	}
}