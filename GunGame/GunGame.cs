using System;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;

namespace Gungame
{
	[PluginDetails(
		author = "Joker119",
		name = "GunGame Gamemode",
		description = "Kill EVERYONE!",
		id = "gungame.gamemode",
		version = "2.1.1",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class GunGame : Plugin
	{
		public Functions Functions { get; private set; }

		public Random Gen = new Random();

		public string[] ValidRanks { get; private set; }

		public Player Winner { get; internal set; }
		
		public bool RoundStarted { get; internal set; }
		public bool Reversed { get; private set; }
		public bool Enabled { get; internal set; }

		public int Health { get; private set; }

		public string Zone { get; internal set; }

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
			AddConfig(new ConfigSetting("gun_reversed", true, true, "If the traditional gungame mode should be reversed."));
			AddConfig(new ConfigSetting("gun_spawn_zone", "lcz", true, "Where you should spawn."));
			AddConfig(new ConfigSetting("gun_health", 100, true, "How much health you will have."));
			AddConfig(new ConfigSetting("gun_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this));

			AddCommands(new string[] { " gungame", "gun" }, new GunGameCommand(this));

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