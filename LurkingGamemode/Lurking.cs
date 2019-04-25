using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using System.Collections.Generic;
using MEC;

namespace LurkingGamemode
{
	[PluginDetails(
		author = "Joker119",
		name = "Lurking in the dark Gamemode",
		description = "Lurking in the Dark Gamemode",
		id = "lurking.gamemode",
		version = "2.0.0-gmm",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Lurking : Plugin
	{
		public Functions Functions { get; private set; }

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; } = false;
		public bool RoundStarted { get; internal set; } = false;
		public bool FlashlightsOnSpawn { get; private set; }

		public int LarryHealth { get; private set; }
		public int DoggoHealth { get; private set; }
		public int LarryCount { get; private set; }
		public int DoggoCount { get; private set; }


		public List<Room> BlackoutRooms = new List<Room>();

		public override void OnDisable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been disbaled.");
		}

		public override void OnEnable()
		{
			Info(Details.name + " v." + Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			AddConfig(new ConfigSetting("lurking_106_num", 2, true, "The number of Larries to spawn"));
			AddConfig(new ConfigSetting("lurking_939_num", 2, true, "The number of 939's to spawn."));
			AddConfig(new ConfigSetting("lurking_106_health", 750, true, "The amount of health Larry should start with."));
			AddConfig(new ConfigSetting("lurking_939_health", 2300, true, "The amount of health Doggo should start with."));
			AddConfig(new ConfigSetting("lurk_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));
			AddConfig(new ConfigSetting("lurking_flashlights", true, true, "If players should spawn with flashlights."));

			AddEventHandlers(new EventsHandler(this), Priority.Normal);

			AddCommands(new string[] { "lurking", "lurk", "litd" }, new LurkingCommand(this));

			GamemodeManager.GamemodeManager.RegisterMode(this, "-1");

			Functions = new Functions(this);

		}

		public void ReloadConfig()
		{
			LarryCount = GetConfigInt("lurking_106_num");
			DoggoCount = GetConfigInt("lurking_939_num");
			LarryHealth = GetConfigInt("lurking_106_health");
			DoggoHealth = GetConfigInt("lurking_939_health");
			ValidRanks = GetConfigList("lurk_gamemode_ranks");
		}
	}
}