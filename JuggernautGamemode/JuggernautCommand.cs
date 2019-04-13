using Smod2.Commands;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;

namespace JuggernautGamemode
{
	class JuggernautCommand : ICommandHandler
	{
		private readonly Juggernaut plugin;

		public JuggernautCommand(Juggernaut plugin) => this.plugin = plugin;

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return "Juggernaut Enabled : " + plugin.Enabled + "\n" +
				"[JUGGERNAUT / JUGG / JUG] HELP \n" +
				"JUGGERNAUT ENABLE \n" +
				"JUGGERNAUT DISABLE \n" +
				"JUGGERNAUT SELECT [PlayerName]";
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
						"Juggernaut Command List",
						"Juggernaut enable - Enables the Juggernaut gamemode.",
						"Juggernaut disable - Disables the Juggernaut gamemode.",
						"Juggernaut select [PlayerName] - Selects the player to be the Juggernaut"
					};
				case "enable":
					plugin.Functions.EnableGamemode();

					return new string[] { "Juggernaut will be enabled for the next round." };
				case "disable":
					plugin.Functions.DisableGamemode();

					return new string[] { "Juggernaut will be disabled for the next round." };
				case "select":
					if (GamemodeManager.GamemodeManager.CurrentMode == plugin && args.Length > 1)
					{
						List<Player> players = plugin.Server.GetPlayers(args[1]);
						Player player;

						if (players == null || players.Count == 0) return new string[] { "Player not found." };

						player = players.OrderBy(ply => ply.Name.Length).First();

						if (player == null)
						{
							return new string[] { " Couldn't get player: " + args[1] };
						}

						plugin.SelectedJugg = player;
						plugin.Info("" + player.Name + " chosen as the Juggernaut!");

						return new string[] { " Player " + player.Name + " selected as the next Juggernaut!" };
					}
					return new string[] { "A player name must be specified, and the gamemode must be Enabled!" };
				default:
					return new string[] { GetUsage() };
			}
		}
	}
}
