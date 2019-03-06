using Smod2.Commands;

namespace Mystery
{
	class MysteryCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "";
		}
		public string GetUsage()
		{
			return "Mystery Enabled : " + Mystery.enabled + "\n"+
                "[mystery / murder] HELP \n" +
                "mystery ENABLE \n" +
                "mystery DISABLE";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length > 0)
			{
				switch (args[0].ToLower())
				{
					case "help":
						return new string[]
						{
							"Mystery Command List \n"+
							"mystery enable - Enables the Murder Mystery gamemode. \n"+
							"mystery disable - Disables the Murder Mystery gamemode. \n"

						};
					case "enable":
						Functions.singleton.EnableGamemode();
						return new string[]
						{
							"Mystery gamemode will be enabled for the next round."
						};
					case "disable":
						Functions.singleton.DisableGamemode();
						return new string[]
						{
							"Mystery gamemode now disabled."
						};
					default:
						return new string[]
						{
							GetUsage()
						};
				}
			}
			return new string[]
			{
				GetUsage()
			};
		}
	}
}