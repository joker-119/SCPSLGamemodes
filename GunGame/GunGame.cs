using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using scp4aiur;
using System.Collections.Generic;
using System;

namespace Gungame
{
	[PluginDetails(
		author = "Joker119",
		name = "GunGame Gamemode",
		description = "Kill EVERYONE!",
		id = "gungame.Gamemode",
		version = "1.7.0",
		SmodMajor = 3,
		SmodMinor = 3,
		SmodRevision = 0
	)]

	public class GunGame : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new System.Random();

		public string[] ValidRanks { get; private set; }

		public List<RoomType> ValidRooms = new List<RoomType>()
			{
				RoomType.CLASS_D_CELLS,
				RoomType.CAFE,
				RoomType.AIRLOCK_00,
				RoomType.AIRLOCK_01,
				RoomType.INTERCOM,
				RoomType.PC_LARGE,
				RoomType.PC_SMALL,
				RoomType.SCP_049,
				RoomType.SCP_096,
				RoomType.SCP_173,
				RoomType.SCP_372,
				RoomType.SCP_939
			 };
		public List<Room> Rooms = new List<Room>();

		public Player Winner { get; internal set; } = null;

		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }
		public bool Reversed { get; private set; }

		public int Health { get; private set; }

		public string Zone { get; internal set; }

		public override void OnDisable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been diisabled.");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been Enabled.");
		}
		public override void Register()
		{
			this.AddConfig(new ConfigSetting("gun_reversed", true, SettingType.BOOL, true, "If the traditional gungame mode should be reversed."));
			this.AddConfig(new ConfigSetting("gun_spawn_zone", "lcz", SettingType.STRING, true, "Where you should spawn."));
			this.AddConfig(new ConfigSetting("gun_health", 100, SettingType.NUMERIC, true, "How much healt you will have."));
			this.AddConfig(new ConfigSetting("gun_gamemode_ranks", new string[] { }, SettingType.LIST, true, "The ranks able to use commands."));

			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

			this.AddCommands(new string[] { " gungame", "gun" }, new GunGameCommand(this));

			Timing.Init(this);

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			Reversed = GetConfigBool("gun_reversed");
			Zone = GetConfigString("gun_spawn_zone");
			Health = GetConfigInt("gun_health");
			ValidRanks = GetConfigList("gun_gamemode_ranks");
		}
	}
}