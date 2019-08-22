using System;
using System.Collections.Generic;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;

namespace Mystery
{
	[PluginDetails(
		author = "Joker119",
		name = "Mystery Gamemode",
		description = "Murder Mystery Gamemode",
		id = "mystery.gamemode",
		version = "2.1.1",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Mystery : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new Random();

		public Dictionary<string, bool> Murd = new Dictionary<string, bool>();

		public string[] ValidRanks { get; private set; }

		public bool RoundStarted { get; internal set; }
		public bool MurdRespawn { get; private set; }
		public bool DetRespawn { get; private set; }
		public bool Enabled { get; internal set; }

		public int MurdererNum { get; private set; }
		public int DetectiveNum { get; private set; }
		public int DetHealth { get; private set; }
		public int CivHealth { get; private set; }
		public int MurdHealth { get; private set; }
		public int MonsterNum { get; private set; }

		public override void OnDisable()
		{
			Info(Details.name + "v." + Details.version + " has been disabled.");
		}

		public override void OnEnable()
		{
			Info(Details.name + "v." + Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			AddConfig(new ConfigSetting("myst_murd_health", 150, true, "How much health murderers should have."));
			AddConfig(new ConfigSetting("myst_civ_health", 100, true, "The amount of health civilians have."));
			AddConfig(new ConfigSetting("myst_det_health", 150, true, "How much health detectives should have."));
			AddConfig(new ConfigSetting("myst_murd_num", 3, true, "The number of murderers to have."));
			AddConfig(new ConfigSetting("myst_det_num", 2, true, "The number of detectives to have."));
			AddConfig(new ConfigSetting("myst_monster_num", 3, true, "The number of monsters that should be in the game."));
			AddConfig(new ConfigSetting("myst_murd_respawn", true, true, "If a random murderer should be respawned."));
			AddConfig(new ConfigSetting("myst_det_respawn", true, true, "If a random Detective should be respawned."));
			AddConfig(new ConfigSetting("myst_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this));

			AddCommands(new string[] { "mystery", "murder" }, new MysteryCommand(this));

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			CivHealth = GetConfigInt("myst_civ_health");
			DetHealth = GetConfigInt("myst_det_health");
			MurdHealth = GetConfigInt("myst_murd_health");
			DetectiveNum = GetConfigInt("myst_det_num");
			MonsterNum = GetConfigInt("myst_monster_num");
			MurdererNum = GetConfigInt("myst_murd_num");
			DetRespawn = GetConfigBool("myst_det_respawn");
			MurdRespawn = GetConfigBool("myst_murd_respawn");
			ValidRanks = GetConfigList("myst_gamemode_ranks");
		}
	}
}