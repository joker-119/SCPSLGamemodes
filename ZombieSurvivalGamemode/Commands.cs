using Smod2.Commands;

namespace ZombielandGamemode
{
	public class ZombieCommand : ICommandHandler
	{
		private readonly Zombie plugin;
		public ZombieCommand(Zombie plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "";

		public string GetUsage() =>
			"Zombie Survival Enabled : " + plugin.Enabled + "\n" +
			"[zombieland / zland / zl] HELP \n" +
			"zland ENABLE \n" +
			"zland DISABLE";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0) return new string[] { GetUsage() };
			if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied!" };

			switch (args[0].ToLower())
			{
				case "help":
					return new string[]
					{
						"Zombie Survival Command List",
						"zland enable - Enables the Zombie Survival Gamemode.",
						"zland disable - Disables the Zombie Survival Gamemode."
					};
				case "enable":
					plugin.Functions.EnableGamemode();
					return new string[] { "Zombieland gamemode now enabled." };
				case "disable":
					plugin.Functions.DisableGamemode();
					return new string[] { "Zombieland gamemode now disabled." };
				default:
					return new string[] { GetUsage() };
			}
		}
	}
}