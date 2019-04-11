using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using System.Collections.Generic;
using System;
using scp4aiur;

namespace MassacreGamemode
{
	[PluginDetails(
		author = "Joker119",
		name = "Massacre of the D-Bois Gamemode",
		description = "Gamemode Template",
		id = "massacre.Gamemode",
		version = "1.8.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]
	public class Massacre : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new System.Random();

		public List<Vector> SpawnLocs = new List<Vector>();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }

		public string SpawnRoom { get; internal set; }

		public Vector SpawnLoc { get; internal set; }

		public int NutHealth { get; private set; }
		public int NutCount { get; private set; }

		public Player Winner { get; internal set; } = null;

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
			AddConfig(new ConfigSetting("mass_spawn_room", "jail", false, true, "Where everyone should spawn."));
			AddConfig(new ConfigSetting("mass_peanut_health", 1, true, "How much health Peanuts spawn with."));
			AddConfig(new ConfigSetting("mass_peanut_count", 3, true, "The number of peanuts selected."));
			AddConfig(new ConfigSetting("mass_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this), Priority.Normal);

			AddCommands(new string[] { "massacre", "motdb", "mascr" }, new MassacreCommand(this));

			Timing.Init(this);

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			SpawnRoom = GetConfigString("mass_spawn_room");
			SpawnLoc = this.Functions.SpawnLoc();
			NutHealth = GetConfigInt("mass_peanut_health");
			NutCount = GetConfigInt("mass_peanut_count");
			ValidRanks = GetConfigList("mass_gamemode_ranks");
		}
	}
}