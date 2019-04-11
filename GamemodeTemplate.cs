using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;

namespace Gamemode
{
	[PluginDetails(
		author = "Mozeman",
		name = "Gamemode Name",
		description = "Gamemode Template",
		id = "moze.gamemode.name",
		version = "1.0",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 2
		)]
	public class Gamemode : Plugin
	{
		public override void OnDisable()
		{
		}

		public override void OnEnable()
		{
			this.Info("Gamemode has loaded");
		}

		public override void Register()
		{
			// Register Events
			// AddEventHandlers(new RoundStartHandler(this), Priority.Highest);
			GamemodeManager.GamemodeManager.RegisterMode(this, "01111111111111111111");
		}
	}
}