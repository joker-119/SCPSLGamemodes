using Smod2.Commands;
using Smod2.API;

namespace Gungame
{
	class GunGameCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "";
		}
		public string GetUsage()
		{
			return "GunGame Enabled : " + GunGame.enabled + "\n"+
			"[gungame / gun] HELP \n"+
			"gun ENABLE \n"+
			"gun DISABLE \n"+
			"gun SELECT [ZONETYPE]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length > 0)
			{
				Player p = sender as Player;
				switch (args[0].ToLower())
				{
					case "help":
						return new string[]
						{
							"GunGame Command List",
							"gun enable - Enables the gamemode.",
							"gun disable - Disables the gamemode.",
							"gun select [zonetype] - selects a specific zone for this round. Acceps lcz, hcz and ent"
						};
					case "enable":
						Functions.singleton.EnableGamemode();
						return new string[] { "Gungame gamemode will be enabled for the next round!"};
					case "disable":
						Functions.singleton.DisableGamemode();
						return new string[] { "Gungame gamemode now disabled."};
					case "debug":
						if (!GunGame.roundstarted)
							return new string[] {"Please wait for the gungame round to begin before using this command."};
						Functions.singleton.ReplaceGun(p);
						return new string[] { "Debugging gun replacement for " + p.Name + "."};
					case "win":
						if (!GunGame.roundstarted)
							return new string[] {"Please wait for the round to begin before using this command."};
						Functions.singleton.AnnounceWinner(p);
						return new string[] {p.Name + " is the winner!"};
					case "select":
						if (args.Length > 1)
						{
							if (GunGame.roundstarted)
								return new string[] {"This command cannot be used once the round has started."};
							if (!GunGame.enabled)
								return new string[] {"Please enable the gamemode before using this command."};
							switch (args[1].ToLower())
							{
								case "lcz":
									GunGame.zone = "lcz";
									return new string[] {"Light Containment Zone selected!"};
								case "hcz":
									GunGame.zone = "hcz";
									return new string[] {"Heavy Containment Zone Selected!"};
								case "ent":
									GunGame.zone = "ent";
									return new string[] {"Entrance zone selected!"};
								default:
									return new string[] {"An invalid zone was specified!"};
							}
						}
						return new string[] {"You must specify a zone!"};
					default:
						return new string[] {GetUsage()};
				}
			}
			return new string[] {GetUsage()};
		}
	}
}