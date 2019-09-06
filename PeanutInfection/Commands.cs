using Smod2.Commands;

namespace PeanutInfection
{
	internal class Commands : ICommandHandler
	{
		private readonly PeanutInfection plugin;
		public Commands(PeanutInfection plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0)
				return new[] { GetUsage() };
			if (!plugin.Functions.IsAllowed(sender))
				return new[] { "Permission denied." };

			switch (args[0].ToLower())
			{
				case "enable":
					plugin.Functions.EnableGamemode();
					return new[] { "Peanut Infection enabled." };
				case "disable":
					plugin.Functions.DisableGamemode();
					return new[] { "Peanut Infection disabled." };
				default:
					return new[] { GetUsage() };
			}
		}

		public string GetUsage() =>
			"[PeanutInfection / infection] HELP \n" + "infection ENABLE \n" + "infection DISABLE";

		public string GetCommandDescription() => "";
	}
}