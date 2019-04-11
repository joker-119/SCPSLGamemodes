using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using scp4aiur;
using System.Collections.Generic;
using System;

namespace Bomber
{
	[PluginDetails(
		author = "Joker119",
		name = "Bomberman Gamemode",
		description = "Run from the bombs!",
		id = "bomberman.Gamemode",
		version = "1.8.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Bomber : Plugin
	{
		public Functions Functions { get; private set; }
		public Random Gen = new System.Random();

		public string[] ValidRanks;

		public bool Enabled { get; internal set; } = false;
		public bool Medkits { get; private set; }
		public bool Warmode { get; internal set; }
		public bool RoundStarted { get; internal set; } = false;

		public int Count { get; internal set; } = 1;
		public int Min { get; private set; }
		public int Max { get; private set; }
		public int Timer { get; internal set; } = 0;

		public string SpawnClass { get; internal set; }
		public float GrenadeMulti { get; private set; }
		public List<Player> players = new List<Player>();

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
			AddConfig(new ConfigSetting("bomb_class", "", true, "The class everyone spawns as. If empty, normal game round."));
			AddConfig(new ConfigSetting("bomb_min", 15, true, "The minimum time before the first drop."));
			AddConfig(new ConfigSetting("bomb_max", 30, true, "The maximum time before the first drop."));
			AddConfig(new ConfigSetting("bomb_medkits", true, true, "If players should spawn with a medkit."));
			AddConfig(new ConfigSetting("bomb_grenade_multi", 0.5f, true, "The number to multiply grenade damage bu."));
			AddConfig(new ConfigSetting("bomb_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this), Priority.Normal);

			AddCommands(new string[] { "bomberman", "bomb" }, new BomberCommand(this));
			Timing.Init(this);

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