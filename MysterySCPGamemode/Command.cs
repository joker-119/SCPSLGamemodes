using Smod2.Commands;

namespace SCP
{
	internal class ScpCommand : ICommandHandler
	{
		private readonly Scp plugin;
		public ScpCommand(Scp plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "";

		public string GetUsage() => "[MysterySCP / SCP] HELP \n" + "scp SELECT \n";
		
		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0) return new string[] { GetUsage() };
			if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

			switch (args[0].ToLower())
			{
				case "help":
					return new string[]
					{
						"Mystery SCP Command List",
						"scp select - Selects the SCP to be spawned."
					};
				case "enable":
					plugin.Functions.EnableGamemode();

					return new string[] { "MysterySCP gamemode will be Enabled for the next round!" };
				case "disable":
					plugin.Functions.DisableGamemode();

					return new string[] { "MysterySCP gamemode now disabled." };
				case "select":
					if (args.Length < 1) return new string[] { "You must specify an SCP role." };

					switch (args[1].ToLower())
					{
						case "939":
							plugin.ScpType = "939";

							return new string[] { "SCP 939 selected." };
						case "106":
							plugin.ScpType = "106";

							return new string[] { "SCP 106 selected." };
						case "173":
							plugin.ScpType = "173";

							return new string[] { "SCP 173 selected." };
						case "096":
							plugin.ScpType = "096";

							return new string[] { "SCP 096 selected." };
						case "049":
							plugin.ScpType = "049";

							return new string[] { "SCP 049 selected." };
						case "random":
							plugin.ScpType = "random";

							return new string[] { "Random SCP type selected." };
						default:
							return new string[] { "Invalid SCP type." };
					}
				default:
					return new string[] { GetUsage() };
			}
		}
	}
}