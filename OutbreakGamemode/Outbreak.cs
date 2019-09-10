using System.Collections.Generic;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;

namespace OutbreakGamemode
{
	[PluginDetails(
		author = "Joker119",
		name = "Outbreak Gamemode",
		description = "Gamemode Template",
		id = "outbreak.gamemode",
		version = "2.4.0",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 1
	)]
	public class Outbreak : Plugin
	{
		public Methods Functions { get; private set; }

		public List<string> Alphas = new List<string>();

		public bool RoundStarted { get; internal set; }
		public bool AlphaDoorDestroy { get; private set; }
		public bool Enabled { get; internal set; }

		public int AlphaHealth { get; private set; }
		public int ChildHealth { get; private set; }
		public int AlphaDamage { get; private set; }
		public int ChildDamage { get; private set; }
		
		public string[] ValidRanks = new string[] {};


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
			AddConfig(new ConfigSetting("outbreak_zombie_health", 3000, true, "The amount of health the starting zombies have."));
			AddConfig(new ConfigSetting("outbreak_child_health", 500, true, "The amount of health child zombies should have."));
			AddConfig(new ConfigSetting("outbreak_alphas_destroy_doors", true, true, "If Alpha zombies should destroy locked doors."));
			AddConfig(new ConfigSetting("outbreak_zombie_damage", 100, true, "The amount of damage the starting zombies deal."));
			AddConfig(new ConfigSetting("outbreak_child_damage", 100, true, "The amount of damage the child zombies should deal."));
			AddConfig(new ConfigSetting("outbreak_gamemode_ranks", new string[] { }, true, "The ranks able to use gamemode commands."));

			AddEventHandlers(new EventsHandler(this));
			AddCommands(new []{"outbreak"}, new Commands(this));

			Functions = new Methods(this);
		}

		public void ReloadConfig()
		{
			AlphaHealth = GetConfigInt("outbreak_zombie_health");
			ChildHealth = GetConfigInt("outbreak_child_health");
			AlphaDamage = GetConfigInt("outbreak_zombie_damage");
			ChildDamage = GetConfigInt("outbreak_child_damage");
			AlphaDoorDestroy = GetConfigBool("outbreak_alphas_destroy_doors");
			GetConfigList("outbreak_gamemode_ranks");
		}
	}
}