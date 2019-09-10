using System;
using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Attributes;

namespace HostageGamemode
{
	[PluginDetails(author = "Joker119", name = "HostageGamemode", id = "hostage.gamemode", description = "", version = "2.4.0",
		configPrefix = "hostage", SmodMajor = 3, SmodMinor = 5, SmodRevision = 1)]

	public class HostageGamemode : Plugin
	{
		public Methods Functions { get; private set; }

		[ConfigOption] public int CriminalCount = 5;
		[ConfigOption] public int HostageCount = 2;
		[ConfigOption] public int CriminalHealth = 150;
		[ConfigOption] public int HostageHealth = 120;
		[ConfigOption] public int SwatHealth = 150;
		[ConfigOption] public float SwatDelay = 120f;
		
		[ConfigOption] public bool Crash = false;

		public Vector CriminalSpawn;
		public Vector PoliceSpawn;
		public bool RoundStarted = false;
		public bool Enabled { get; internal set; }
		public string[] ValidRanks = new string[] {};
		public List<int> Hostages = new List<int>();
		public Random Gen = new Random();

		public override void Register()
		{
			AddEventHandlers(new EventHandlers(this));
			AddCommands(new[] { "hostage" }, new Commands(this));

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