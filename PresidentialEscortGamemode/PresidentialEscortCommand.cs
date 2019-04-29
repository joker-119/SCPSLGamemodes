using System.Collections.Generic;
using System.Linq;
using Smod2.API;
using Smod2.Commands;

namespace PresidentialEscortGamemode
{
    internal class PresidentialEscortCommand : ICommandHandler
    {
        private readonly PresidentialEscort plugin;

        public PresidentialEscortCommand(PresidentialEscort plugin) => this.plugin = plugin;

        public string GetCommandDescription() => "";

        public string GetUsage() =>
            "[presidentialescort / presidential / escort / pe] HELP \n" +
            "presidential SELECT";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

            switch (args[0].ToLower())
            {
                case "help":
                    return new string[]
                    {
                            "Presidential Escort Command List",
                            "escort select - Selects who will be the next VIP."
                    };
                case "select":
                    {
                        if (args.Length <= 1) return new string[] { "Invalid player name." };

                        List<Player> players = plugin.Server.GetPlayers(args[1]);

                        if (players == null || players.Count == 0) return new string[] { "Player not found!" };

                        Player player = players.OrderBy(ply => ply.Name.Length).First();

                        plugin.Vip = player;

                        return new string[] { player.Name + " selected as the next VIP!" };
                    }
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}
