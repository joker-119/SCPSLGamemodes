using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using System.Collections.Generic;
using scp4aiur;

namespace ZombielandGamemode
{
	[PluginDetails(
		author = "Joker119",
		name = "Zombieland Gamemode",
		description = "Gamemode Template",
		id = "zombieland.Gamemode",
		version = "1.7.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]
	public class Zombieland : Plugin
	{
		public Methods Functions { get; private set; }

		public List<string> Alphas = new List<string>();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }
		public bool AlphaDoorDestroy { get; private set; }

		public int AlphaHealth { get; private set; }
		public int ChildHealth { get; private set; }
		public int AlphaDamage { get; private set; }
		public int ChildDamage { get; private set; }


		public override void OnDisable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been disabled.");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been Enabled.");
		}

		public override void Register()
		{
			this.AddConfig(new ConfigSetting("zombieland_zombie_health", 3000, true, "The amount of health the starting zombies have."));
			this.AddConfig(new ConfigSetting("zombieland_child_health", 500, true, "The amoutn of health child zombies should have."));
			this.AddConfig(new ConfigSetting("zombieland_alphas_destroy_doors", true, true, "If Alpha zombies should destroy locked doors."));
			this.AddConfig(new ConfigSetting("zombieland_zombie_damage", 100, true, "The amount of damage the starting zombies deal."));
			this.AddConfig(new ConfigSetting("zombieland_child_damage", 100, true, "The amount of damage the child zombies should deal."));
			this.AddConfig(new ConfigSetting("zombie_gamemode_ranks", new string[] { }, true, "The ranks able to use gamemode commands."));

			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

			this.AddCommands(new string[] { "zombie", "zombieland", "zl" }, new ZombielandCommand(this));

			Timing.Init(this);

			Functions = new Methods(this);
		}

		public void ReloadConfig()
		{
			AlphaHealth = GetConfigInt("zombieland_zombie_health");
			ChildHealth = GetConfigInt("zombieland_child_health");
			AlphaDamage = GetConfigInt("zombieland_zombie_damage");
			ChildDamage = GetConfigInt("zombieland_child_damage");
			AlphaDoorDestroy = GetConfigBool("zombieland_alphas_destroy_doors");
			ValidRanks = GetConfigList("zombie_gamemode_ranks");
		}
	}
}