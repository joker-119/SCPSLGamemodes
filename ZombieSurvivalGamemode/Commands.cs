using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace ZombieSurvival
{
	public class ZombieCommand : ICommandHandler
	{
		private readonly Zombie plugin;
		public ZombieCommand(Zombie plugin) => this.plugin = plugin;

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return "Zombie Survival Enabled : " + plugin.Enabled + "\n" +
				"[zombiesurvival / zsurv / zs / za] HELP \n" +
				"zsurv ENABLE \n" +
				"zsurv DISABLE";
		}

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
						"zsurv enable - Enables the Zombie Survival Gamemode.",
						"zsurv disable - Disables the Zombie Survival Gamemode."
					};
				case "enable":
					plugin.Functions.EnableGamemode();
					return new string[] { "Zombie Survival gamemode now enabled." };
				case "disable":
					plugin.Functions.DisableGamemode();
					return new string[] { "Zombie Survival gamemode now disabled." };
				default:
					return new string[] { GetUsage() };
			}
		}
	}
}