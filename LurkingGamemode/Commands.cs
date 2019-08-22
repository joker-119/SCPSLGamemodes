using Smod2.Commands;

namespace LurkingGamemode
{
	internal class Commands : ICommandHandler
	{
		private readonly Lurking plugin;
		public Commands(Lurking plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0) return new string[] { GetUsage() };
			if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };
			
			switch (args[0].ToLower())
			{
				case "help":
					return new[]
					{
						"Lurking in the Dark command list:", 
						"lurk enable - Enables the gamemode.",
						"lurk disable - Disables the gamemode."
					};
				case "enable":
					plugin.Functions.EnableGamemode();
					return new[] { "Lurking in the Dark gamemode enabled." };
				case "disable":
					plugin.Functions.DisableGamemode();
					return new[] { "Lurking in the Dark gamemode disabled." };
				default:
					return new[] { GetUsage() };
			}
		}

		public string GetUsage() => "A command argument must be used.";

		public string GetCommandDescription() => "";
	}
}