using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Attributes;
using System;
using System.Collections.Generic;

namespace SCPRouletteGamemode
{
	[PluginDetails(author = "Joker119", name = "SCPRouletteGamemode", id = "roulette.gamemode", description = "", version = "2.3.0",
		configPrefix = "roulette", SmodMajor = 3, SmodMinor = 5, SmodRevision = 1)]

	public class ScpRouletteGamemode : Plugin
	{
		[ConfigOption] public float Delay;
		public Methods Functions { get; private set; }

		public bool RoundStarted = false;
		public bool Enabled { get; internal set; }
		public string[] ValidRanks = new string[] {};

		public Random Gen = new Random();
		
		public List<Role> ScpRoles = new List<Role> { Role.SCP_049, Role.SCP_049_2, Role.SCP_096, Role.SCP_106, Role.SCP_173, Role.SCP_939_53 };
		
		public override void Register()
		{
			AddEventHandlers(new EventHandlers(this));
			AddCommands(new []{"roulette"}, new Commands(this));

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