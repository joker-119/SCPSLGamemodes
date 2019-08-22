using Smod2.Commands;

namespace OutbreakGamemode
{
	internal class Commands : ICommandHandler
	{
		private readonly Outbreak plugin;
		public Commands(Outbreak plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0) return new string[] { GetUsage() };
			if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };
			
			switch (args[0].ToLower())
			{
				case "help":
					return new[]
					{
						"Oubreak in the Dark command list:", 
						"outbreak enable - Enables the gamemode.",
						"outbreak disable - Disables the gamemode."
					};
				case "enable":
					plugin.Functions.EnableGamemode();
					return new[] { "Outbreak gamemode enabled." };
				case "disable":
					plugin.Functions.DisableGamemode();
					return new[] { "Outbreak gamemode disabled." };
				default:
					return new[] { GetUsage() };
			}
		}

		public string GetUsage() => "A command argument must be used.";

		public string GetCommandDescription() => "";
	}
}