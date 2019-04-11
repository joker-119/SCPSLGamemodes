using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Events;
using Smod2.Attributes;
using System;
using System.Collections.Generic;
using MEC;

namespace ZombieSurvival
{
	[PluginDetails(
		author = "Joker119",
		name = "Zombie Survival gamemode.",
		description = "3 NTF vs a horde of zombies.",
		id = "zombiesurvival.Gamemode",
		version = "1.8.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Zombie : Plugin
	{
		public Methods Functions { get; private set; }

		public Random Gen = new System.Random();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }

		public int NTFAmmo { get; private set; }
		public int NTFHealth { get; private set; }
		public int MaxNTFCount { get; private set; }
		public int ZHealth { get; private set; }
		public int MaxZRespawn { get; private set; }

		public float CarePackageTimer { get; private set; }
		public float AmmoTimer { get; private set; }
		public float RoundTimer { get; private set; }
		public float ZDamageMultiplier { get; private set; }

		public List<ItemType> NTFItems = new List<ItemType>();
		public List<Room> Rooms = new List<Room>();
		public List<Player> NTF = new List<Player>();

		public ItemType CarePackage { get; private set; }

		public Vector NTFSpawn { get; private set; }


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
			AddConfig(new ConfigSetting("zsurv_ntf_ammo", 250, true, "The amount of ammo NTF start with."));
			AddConfig(new ConfigSetting("zsurv_ntf_health", 150, true, "The amount of health NTF start with."));
			AddConfig(new ConfigSetting("zsurv_ntf_max", 3, true, "The maximum number of NTF that will spawn."));
			AddConfig(new ConfigSetting("zsurv_ntf_items", new int[] { 8, 14, 14, 14, 14, 20 }, true, "The starting items for NTF"));

			AddConfig(new ConfigSetting("zsurv_zombie_health", 1500, true, "The health Zombies spawn with."));
			AddConfig(new ConfigSetting("zsurv_zombie_maxrespawn", -1, true, "The maximum number of zombies that can respawn."));
			AddConfig(new ConfigSetting("zsurv_zombie_resistance", 0.8f, true, "The percent amount of damage dealt to zombies."));

			AddConfig(new ConfigSetting("zsurv_carepackage_timer", 180, true, "The time in seconds between carepackages."));
			AddConfig(new ConfigSetting("zsurv_carepackage_item", "logicer", true, "The item spawned by the carepackage."));

			AddConfig(new ConfigSetting("zsurv_ammo_timer", 180, true, "The amount of time between ammo drops."));
			AddConfig(new ConfigSetting("zsurv_round_timer", 600, true, "The amount of time the round will last."));

			AddConfig(new ConfigSetting("zsurv_gamemode_ranks", new string[] { }, true, "The ranks able to use gamemode commands."));

			AddEventHandlers(new EventHandler(this), Priority.Normal);

			AddCommands(new string[] { "zombiesurvival", "zsurv", "zs", "za" }, new ZombieCommand(this));

			Functions = new Methods(this);
		}

		public void ReloadConfig()
		{
			NTFAmmo = GetConfigInt("zsurv_ntf_ammo");
			NTFHealth = GetConfigInt("zsurv_ntf_health");
			MaxNTFCount = GetConfigInt("zsurv_ntf_max");
			ZHealth = GetConfigInt("zsurv_zombie_health");
			MaxZRespawn = GetConfigInt("zsurv_zombie_maxrespawn");
			ZDamageMultiplier = GetConfigFloat("zsurv_zombie_resistance");
			CarePackageTimer = GetConfigFloat("zsurv_carepackage_timer");
			AmmoTimer = GetConfigFloat("zsurv_ammo_timer");
			RoundTimer = GetConfigFloat("zsurv_round_timer");
			CarePackage = (ItemType)GetConfigInt("zsurv_carepackage_item");
			ValidRanks = GetConfigList("zsurv_gamemode_ranks");
			Functions.Get079Rooms();
			NTFSpawn = Functions.Spawn();

			int[] items = GetConfigIntList("zsurv_ntf_items");

			foreach (int i in items)
			{
				if (Enum.IsDefined(typeof(ItemType), i))
					NTFItems.Add((ItemType)i);
			}
		}
	}
}