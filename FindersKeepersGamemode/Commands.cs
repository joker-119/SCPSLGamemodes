using Smod2.API;
using Smod2.Commands;

namespace FindersKeepersGamemode
{
	internal class Commands : ICommandHandler
	{
		private readonly FindersKeepersGamemode plugin;
		public Commands(FindersKeepersGamemode plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0)
				return new[] { GetUsage() };
			if (!plugin.Functions.IsAllowed(sender))
				return new[] { "Permission denied." };

			switch (args[0].ToLower())
			{
				case "help":
					return new[]
					{
						"Finder's Keepers command list", "fk enable - Enables the gamemode.",
						"fk disable - Disables the gamemode.",
					};
				case "enable":
					plugin.Functions.EnableGamemode();
					return new[] { "Finder's Keepers will be enabled for the next round!" };
				case "disable":
					plugin.Functions.DisableGamemode();
					return new[] { "Finder's Keepers now disabled." };
				case "spawncoin":
					// do stuff
				default:
					return new[] { GetUsage() };
			}
		}

		public string GetUsage() => 
			"[finders / fk] HELP \n" +
			"fk ENABLE \n" + 
			"fk DISABLE"
		;

		public string GetCommandDescription() => "";
	}
}