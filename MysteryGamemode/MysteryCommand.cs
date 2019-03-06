using Smod2.Commands;
using Smod2.API;

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
					case "spawn":
						if (Mystery.enabled && Mystery.roundstarted && args.Length > 2)
						{
							Player player = GetPlayerFromString.GetPlayer(args[1]);
							if (player == null)
							{
								return new string[] { "Couldn't get player: " + args[1]};
							}
							switch (args[2].ToLower())
							{
								case "det":
								{
									Functions.singleton.SpawnDet(player);
									return new string[]{"Player " + player.Name + " spawned as a Detective."};
								}
								case "murd":
								{
									Functions.singleton.SpawnMurd(player);
									return new string[]{"Player " + player.Name + " spawned as a Murderer."};
								}
								case "civ":
								{
									Functions.singleton.SpawnCiv(player);
									return new string[]{"Player " + player.Name + " spawned as a Civilian."};
								}
							}
						}
						return new string[] {"This command must specify a player and role, and can only be used after the round has begun."};
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