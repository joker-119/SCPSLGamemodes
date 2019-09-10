using System;
using Smod2;
using Smod2.Config;
using Smod2.Attributes;

namespace PeanutInfection
{
	[PluginDetails(author = "Joker119", name = "PeanutInfection", id = "joker.PeanutInfection", description = "", version = "2.4.0",
		configPrefix = "", SmodMajor = 3, SmodMinor = 5, SmodRevision = 1)]

	public class PeanutInfection : Plugin
	{
		public Methods Functions { get; private set; }
		public bool Enabled = false;
		public bool RoundStarted = false;
		public Random Gen = new Random();
		[ConfigOption] public string[] ValidRanks = new string[] { };
		[ConfigOption] public bool RespawnOnKiller = true;

		public override void Register()
		{
			AddEventHandlers(new EventHandlers(this));
			AddCommands(new[] { "peanutinfection", "infection" }, new Commands(this));

			Functions = new Methods(this);
		}

		public override void OnEnable()
		{
			Info(Details.name + " v." + Details.version + " has been enabled.");
		}

		public override void OnDisable()
		{
			Info(Details.name + " has been disabled.");
		}
	}
}