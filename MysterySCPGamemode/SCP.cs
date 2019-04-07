using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Events;
using Smod2.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;
using MEC;

namespace SCP
{
	[PluginDetails(
		author = "Joker119",
		name = "Mystery SCP Gamemode",
		description = "All SCP's are a single SCP type.",
		id = "scp.Gamemode",
		version = "1.7.5",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class SCP : Plugin
	{
		public Methods Functions { get; private set; }

		public Random Gen = new System.Random();

		public string[] ValidRanks { get; private set; }

		public bool Enabled { get; internal set; } = false;
		public bool RoundStarted { get; internal set; } = false;

		public string SCPType { get; internal set; } = "random";

		public Role role { get; internal set; }

		public override void OnDisable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been disabled.");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " v." + this.Details.version + " has been enabled.");
		}

		public override void Register()
		{
			this.AddConfig(new ConfigSetting("scp_gamemode_ranks", new string[] { }, true, "The roles able to use gamemode commands."));

			this.AddEventHandlers(new EventHandler(this), Priority.Normal);

			this.AddCommands(new string[] { "mysteryscp", "scp" }, new SCPCommand(this));

			Functions = new Methods(this);
		}

		public void ReloadConfig()
		{
			ValidRanks = GetConfigList("scp_gamemode_ranks");
			role = Functions.SCPType();
		}
	}
}