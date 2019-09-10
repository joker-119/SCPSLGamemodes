using System;
using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;

namespace Bomber
{
	[PluginDetails(
		author = "Joker119",
		name = "Bomberman Gamemode",
		description = "Run from the bombs!",
		id = "bomberman.gamemode",
		version = "2.3.1",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 1
	)]

	public class Bomber : Plugin
	{
		public Functions Functions { get; private set; }
		public Random Gen = new Random();

		public string[] ValidRanks;

		public bool Medkits { get; private set; }
		public bool Warmode { get; internal set; }
		public bool RoundStarted { get; internal set; }
		public bool Enabled { get; internal set; }

		public int Count { get; internal set; } = 1;
		public int Min { get; private set; }
		public int Max { get; private set; }
		public int Timer { get; internal set; }

		public string SpawnClass { get; internal set; }
		public float GrenadeMulti { get; private set; }
		public List<Player> Players = new List<Player>();

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
			AddConfig(new ConfigSetting("bomb_class", "", true, "The class everyone spawns as. If empty, normal game round."));
			AddConfig(new ConfigSetting("bomb_min", 15, true, "The minimum time before the first drop."));
			AddConfig(new ConfigSetting("bomb_max", 30, true, "The maximum time before the first drop."));
			AddConfig(new ConfigSetting("bomb_medkits", true, true, "If players should spawn with a medkit."));
			AddConfig(new ConfigSetting("bomb_grenade_multi", 0.5f, true, "The number to multiply grenade damage bu."));
			AddConfig(new ConfigSetting("bomb_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this));

			AddCommands(new string[] { "bomberman", "bomb" }, new BomberCommand(this));

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			SpawnClass = GetConfigString("bomb_class");
			Medkits = GetConfigBool("bomb_medkits");
			Min = GetConfigInt("bomb_min");
			Max = GetConfigInt("bomb_max");
			GrenadeMulti = GetConfigFloat("bomb_grenade_multi");
			ValidRanks = GetConfigList("bomb_gamemode_ranks");
		}
	}
}