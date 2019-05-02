using System;
using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;

namespace ZombieSurvival
{
	[PluginDetails(
		author = "Joker119",
		name = "Zombie Survival gamemode.",
		description = "3 NTF vs a horde of zombies.",
		id = "zombiesurvival.gamemode",
		version = "2.0.2-gmm",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Zombie : Plugin
	{
		public Methods Functions { get; private set; }

		public Random Gen = new Random();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }

		public int NtfAmmo { get; private set; }
		public int NtfHealth { get; private set; }
		public int MaxNtfCount { get; private set; }
		public int ZHealth { get; private set; }

		public float CarePackageTimer { get; private set; }
		public float AmmoTimer { get; private set; }
		public float RoundTimer { get; private set; }
		public float ZDamageMultiplier { get; private set; }

		public List<ItemType> NtfItems = new List<ItemType>();
		public List<Room> Rooms = new List<Room>();
		public List<Player> Ntf = new List<Player>();

		public ItemType CarePackage { get; private set; }

		public Vector NtfSpawn { get; private set; }


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
			AddConfig(new ConfigSetting("zsurv_zombie_resistance", 0.8f, true, "The percent amount of damage dealt to zombies."));

			AddConfig(new ConfigSetting("zsurv_carepackage_timer", 180f, true, "The time in seconds between carepackages."));
			AddConfig(new ConfigSetting("zsurv_carepackage_item", "logicer", true, "The item spawned by the carepackage."));

			AddConfig(new ConfigSetting("zsurv_ammo_timer", 180f, true, "The amount of time between ammo drops."));
			AddConfig(new ConfigSetting("zsurv_round_timer", 600f, true, "The amount of time the round will last."));

			AddConfig(new ConfigSetting("zsurv_gamemode_ranks", new string[] { }, true, "The ranks able to use gamemode commands."));

			AddEventHandlers(new EventHandler(this));

			AddCommands(new string[] { "zombiesurvival", "zsurv", "zs", "za" }, new ZombieCommand(this));

			Functions = new Methods(this);

			GamemodeManager.GamemodeManager.RegisterMode(this);
		}

		public void ReloadConfig()
		{
			NtfAmmo = GetConfigInt("zsurv_ntf_ammo");
			NtfHealth = GetConfigInt("zsurv_ntf_health");
			MaxNtfCount = GetConfigInt("zsurv_ntf_max");
			ZHealth = GetConfigInt("zsurv_zombie_health");
			ZDamageMultiplier = GetConfigFloat("zsurv_zombie_resistance");
			CarePackageTimer = GetConfigFloat("zsurv_carepackage_timer");
			AmmoTimer = GetConfigFloat("zsurv_ammo_timer");
			RoundTimer = GetConfigFloat("zsurv_round_timer");
			CarePackage = (ItemType)GetConfigInt("zsurv_carepackage_item");
			ValidRanks = GetConfigList("zsurv_gamemode_ranks");
			Functions.Get079Rooms();
			NtfSpawn = Functions.Spawn();

//			int[] items = GetConfigIntList("zsurv_ntf_items");
//
//			foreach (int i in items)
//				if (Enum.IsDefined(typeof(ItemType), i))
//					NtfItems.Add((ItemType)i);

			NtfItems.Add((ItemType)8);
			NtfItems.Add((ItemType)20);
			NtfItems.Add((ItemType)14);
			NtfItems.Add((ItemType)14);
			NtfItems.Add((ItemType)14);
			NtfItems.Add((ItemType)14);
			
		}
	}
}