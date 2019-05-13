using Smod2;
using Smod2.Config;
using Smod2.Attributes;
using Smod2.API;
using System.Collections.Generic;

namespace RisingLavaGamemode
{
	[PluginDetails(author = "Joker119", name = "RisingLavaGamemode", id = "lava.gamemode", description = "", version = "2.1.1-gmm",
		configPrefix = "rlava", SmodMajor = 3, SmodMinor = 4, SmodRevision = 0)]

	public class RisingLavaGamemode : Plugin
	{
		public Methods Functions { get; private set; }

		[ConfigOption] public float LczDelay = 300f;
		[ConfigOption] public float HczDelay = 150f;
		[ConfigOption] public float EntDelay = 120f;
		
		public List<Room> Rooms = new List<Room>();
		public bool RoundStarted = false;

		public override void Register()
		{
			AddEventHandlers(new EventHandlers(this));
			AddCommands(new[] { "risinglava", "rising", "lava" }, new Commands(this));

			Functions = new Methods(this);
			
			GamemodeManager.GamemodeManager.RegisterMode(this);
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