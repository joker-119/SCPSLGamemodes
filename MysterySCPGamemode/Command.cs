using Smod2.Commands;

namespace SCP
{
	class SCPCommand : ICommandHandler
	{
		private readonly SCP plugin;
		public SCPCommand(SCP plugin) => this.plugin = plugin;

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return "Mystery SCP Enavled: " + plugin.Enabled + "\n" +
				"[MysterySCP / SCP] HELP \n" +
				"scp ENABLE \n" +
				"scp DISABLE";
		}

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
						"scp enable - Enables the Mystery SCP gamemode.",
						"scp disable - Disables the Mystery SCP gamemode."
					};
				case "enable":
					plugin.Functions.EnableGamemode();

					return new string[] { "Mystery SCP gamemode enabled." };
				case "disable":
					plugin.Functions.DisableGamemode();

					return new string[] { "Mystery SCP gamemode disabled." };
				case "select":
					if (args.Length < 1) return new string[] { "You must specify an SCP role." };

					switch (args[1].ToLower())
					{
						case "939":
							plugin.SCPType = "939";

							return new string[] { "SCP 939 selected." };
						case "106":
							plugin.SCPType = "106";

							return new string[] { "SCP 106 selected." };
						case "173":
							plugin.SCPType = "173";

							return new string[] { "SCP 173 selected." };
						case "096":
							plugin.SCPType = "096";

							return new string[] { "SCP 096 selected." };
						case "049":
							plugin.SCPType = "049";

							return new string[] { "SCP 049 selected." };
						case "random":
							plugin.SCPType = "random";

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