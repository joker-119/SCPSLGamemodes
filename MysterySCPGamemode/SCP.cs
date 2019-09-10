using System;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;

namespace SCP
{
	[PluginDetails(
		author = "Joker119",
		name = "Mystery SCP Gamemode",
		description = "All SCP's are a single SCP type.",
		id = "scp.gamemode",
		version = "2.3.1",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 1
	)]

	public class Scp : Plugin
	{
		public Methods Functions { get; private set; }

		public Random Gen = new Random();

		public string[] ValidRanks { get; private set; }

		public bool RoundStarted { get; internal set; }
		public bool Enabled { get; internal set; }

		public string ScpType { get; internal set; } = "random";

		public Role Role { get; private set; }

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
			AddConfig(new ConfigSetting("scp_gamemode_ranks", new string[] { }, true, "The roles able to use gamemode commands."));

			AddEventHandlers(new EventHandler(this));
			
			AddCommands(new string[] { "mysteryscp", "scp" }, new ScpCommand(this));

			Functions = new Methods(this);
		}

		public void ReloadConfig()
		{
			ValidRanks = GetConfigList("scp_gamemode_ranks");
			Role = Functions.ScpType();
		}
	}
}