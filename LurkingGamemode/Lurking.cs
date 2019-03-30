using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using System.Collections.Generic;
using scp4aiur;

namespace LurkingGamemode
{
	[PluginDetails(
		author = "Joker119",
		name = "Lurking in the dark Gamemode",
		description = "Lurking in the Dark Gamemode",
		id = "lurking.Gamemode",
		version = "1.7.0",
		SmodMajor = 3,
		SmodMinor = 3,
		SmodRevision = 0
	)]

	public class Lurking : Plugin
	{
		public Functions Functions { get; private set; }

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; } = false;
		public bool RoundStarted { get; internal set; } = false;

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
			this.Info(this.Details.name + " v." + this.Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			this.AddConfig(new ConfigSetting("lurking_106_num", 2, SettingType.NUMERIC, true, "The number of Larries to spawn"));
			this.AddConfig(new ConfigSetting("lurking_939_num", 2, SettingType.NUMERIC, true, "The number of 939's to spawn."));
			this.AddConfig(new ConfigSetting("lurking_106_health", 750, SettingType.NUMERIC, true, "The amount of health Larry should start with."));
			this.AddConfig(new ConfigSetting("lurking_939_health", 2300, SettingType.NUMERIC, true, "The amount of health Doggo should start with."));
			this.AddConfig(new ConfigSetting("lurk_gamemode_ranks", new string[] { }, SettingType.LIST, true, "The ranks able to use commands."));

			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

			this.AddCommands(new string[] { "lurking", "lurk", "litd" }, new LurkingCommand(this));

			Timing.Init(this);

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