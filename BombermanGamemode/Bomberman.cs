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
		id = "bomberman.gamemode",
		version = "1.5.0",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 0
	)]

	public class Bomber : Plugin
	{
		internal static Bomber singleton;
		public static Random gen = new System.Random();
		public static bool
			enabled = false,
			medkits,
			warmode,
			roundstarted = false;
		public static int 
			count = 1,
			min,
			timer = 0,
			max;
		public static string spawn_class;
		public static float grenade_multi;

		public override void OnDisable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " has been diisabled.");
        }

        public override void OnEnable()
        {
            singleton = this;
            this.Info(this.Details.name + " v." + this.Details.version + " has been enabled.");
        }
		public override void Register()
		{
			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
			this.AddCommands(new string[] { "bomberman", "bomb" }, new BomberCommand());
			this.AddConfig(new ConfigSetting("bomb_class", "", SettingType.STRING, true, "The class everyone spawns as. If empty, normal game round."));
			this.AddConfig(new ConfigSetting("bomb_min", 15, SettingType.NUMERIC, true, "The minimum time before the first drop."));
			this.AddConfig(new ConfigSetting("bomb_max", 30, SettingType.NUMERIC, true, "The maximum time before the first drop."));
			this.AddConfig(new ConfigSetting("bomb_medkits", true, SettingType.BOOL, true, "If players should spawn with a medkit."));
			this.AddConfig(new ConfigSetting("bomb_grenade_multi", 0.5f, SettingType.FLOAT, true, "The number to multiply grenade damage bu."));
			Timing.Init(this);
			new Functions(this);
		}
	}
}