using System.Collections.Generic;
using System.Linq;
using Smod2.API;
using Smod2.Commands;

namespace JuggernautGamemode
{
	internal class JuggernautCommand : ICommandHandler
	{
		private readonly Juggernaut plugin;

		public JuggernautCommand(Juggernaut plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "";

		public string GetUsage() =>
			"[JUGGERNAUT / JUGG / JUG] HELP \n" +
			"JUGGERNAUT SELECT [PlayerName]";

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
						"Juggernaut select [PlayerName] - Selects the player to be the Juggernaut"
					};
				case "select":
					if (GamemodeManager.GamemodeManager.CurrentMode != plugin || args.Length <= 1)
						return new string[] { "A player name must be specified, and the gamemode must be Enabled!" };
					
					List<Player> players = plugin.Server.GetPlayers(args[1]);
					Player player;

					if (players == null || players.Count == 0) return new string[] { "Player not found." };

					player = players.OrderBy(ply => ply.Name.Length).First();

					if (player == null) return new string[] { " Couldn't get player: " + args[1] };

					plugin.SelectedJugg = player;
					plugin.Info("" + player.Name + " chosen as the Juggernaut!");

					return new string[] { " Player " + player.Name + " selected as the next Juggernaut!" };
				default:
					return new string[] { GetUsage() };
			}
		}
	}
}
