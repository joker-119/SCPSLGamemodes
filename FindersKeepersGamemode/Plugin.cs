using System;
using System.Collections.Generic;
using MEC;
using Smod2;
using Smod2.API;
using Smod2.Config;
using Smod2.Attributes;

namespace FindersKeepersGamemode
{
	[PluginDetails(author = "Joker119", name = "FindersKeepersGamemode", id = "joker.FindersKeepersGamemode", description = "", version = "2.4.0",
		configPrefix = "fkg", SmodMajor = 3, SmodMinor = 5, SmodRevision = 1)]

	public class FindersKeepersGamemode : Plugin
	{
		public Methods Functions { get; private set; }
		public List<Room> Scp079Rooms = new List<Room>();
		public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public bool Enabled = false;
		public bool RoundStarted = false;
		public string[] ValidRanks = new string[] {};
		public List<Player> Winners = new List<Player>();
		public Random Gen = new Random();

		[ConfigOption] public float StartTimer = 30f;
		[ConfigOption] public int CoinCount = 3;

		public override void Register()
		{
			AddEventHandlers(new EventHandlers(this));
			AddCommands(new[] { "fk", "finders" }, new Commands(this));

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