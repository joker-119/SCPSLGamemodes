using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using UnityEngine;
using System.Collections.Generic;
using MEC;
using System;

namespace JuggernautGamemode
{
	[PluginDetails(
		author = "Mozeman",
		name = "Juggernaut Gamemode",
		description = "Gamemode Template",
		id = "juggernaut.gamemode",
		version = "2.0.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
		)]
	public class Juggernaut : Plugin
	{
		public Functions Functions { get; private set; }

		public System.Random Gen = new System.Random();

		public string[] ValidRanks { get; private set; }

		public bool NTFDisarmer { get; private set; }
		public bool JuggInfiniteNades { get; private set; }
		public bool Enabled { get; internal set; }
		public bool RoundStarted { get; internal set; }

		public int JuggBase { get; private set; }
		public int JuggIncrease { get; private set; }
		public int JuggGrenades { get; private set; }
		public int NTFAmmo { get; private set; }
		public int NTFHealth { get; private set; }
		public int JuggHealth { get; internal set; }

		public Player Jugg { get; internal set; } = null;
		public Player Activator { get; internal set; } = null;
		public Player SelectedJugg { get; internal set; } = null;

		public float CriticalDamage { get; private set; }

		public string[] JuggernautPrevRank = new string[2];

		public HealthBar HealthBarType { get; private set; }
		public enum HealthBar
		{
			Raw,
			Percentage,
			Bar
		}

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
			AddConfig(new ConfigSetting("juggernaut_base_health", 500, true, "The amoutn of base health the Juggernaut starts with."));
			AddConfig(new ConfigSetting("juggernaut_increase_amount", 500, true, "The amount of extra HP a Jugg gets for each additional player."));
			AddConfig(new ConfigSetting("juggernaut_jugg_grenades", 6, true, "The number of grenades the Jugg should start with."));
			AddConfig(new ConfigSetting("juggernaut_ntf_disarmer", false, true, "Wether or not NTF should spawn with Disarmers."));
			AddConfig(new ConfigSetting("juggernaut_ntf_ammo", 272, true, "The amount of ammo NTF Commanders should spawn with."));
			AddConfig(new ConfigSetting("juggernaut_ntf_health", 150, true, "The amount of health the first wave of NTF should have."));
			AddConfig(new ConfigSetting("juggernaut_critical_damage", (float)0.15, true, "The amount of critical damage the Juggernaut should recieve."));
			AddConfig(new ConfigSetting("juggernaut_infinite_jugg_nades", false, true, "If the Juggernaut should have infinite grenades."));
			AddConfig(new ConfigSetting("juggernaut_health_bar_type", "bar", false, true, "Type of Health Bar to use for Juggernaut"));
			AddConfig(new ConfigSetting("jugg_gamemode_ranks", new string[] { }, true, "The ranks able to use commands."));

			AddEventHandlers(new EventsHandler(this), Priority.Normal);

			AddCommands(new string[] { "jug", "jugg", "juggernaut" }, new JuggernautCommand(this));

			Functions = new Functions(this);
		}

		public void ReloadConfig()
		{
			JuggBase = GetConfigInt("juggernaut_base_health");
			JuggIncrease = GetConfigInt("juggernaut_increase_amount");
			NTFDisarmer = GetConfigBool("juggernaut_ntf_disarmer");
			JuggGrenades = GetConfigInt("juggernaut_jugg_grenades");
			NTFHealth = GetConfigInt("juggernaut_ntf_health");
			CriticalDamage = GetConfigFloat("juggernaut_critical_damage");
			JuggInfiniteNades = GetConfigBool("juggernaut_infinite_jugg_nades");
			ValidRanks = GetConfigList("jugg_gamemode_ranks");

			string type = GetConfigString("juggernaut_health_bar_type");
			switch (type.ToLower().Trim())
			{
				case "bar":
					this.Debug("Drawn Bar Health Bar Selected");
					HealthBarType = HealthBar.Bar; break;
				case "percent":
				case "percentage":
					this.Debug("Percentage Health Bar Selected");
					HealthBarType = HealthBar.Percentage; break;
				case "raw":
				default:
					this.Debug("Raw Health Bar Selected");
					HealthBarType = HealthBar.Raw; break;
			}
		}
	}
}