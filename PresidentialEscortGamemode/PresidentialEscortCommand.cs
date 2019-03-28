using System.Linq;
using Smod2.Commands;
using System.Collections.Generic;
using Smod2.API;

namespace PresidentialEscortGamemode
{
    class PresidentialEscortCommand : ICommandHandler
    {
        private readonly PresidentialEscort plugin;

        public PresidentialEscortCommand(PresidentialEscort plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Presidential Escort Enabled : " + plugin.Enabled + "\n" +
                "[presidentialescort / presidential / escort / pe] HELP \n" +
                "presidential ENABLE \n" +
                "presidential DISABLE";
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
                            "Presidential Escort Command List",
                            "Presidential Escort enable - Enables the Presidential Escort gamemode.",
                            "Presidential Escort disable - Disables the Presidential Escort gamemode."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Presidential Escort gamemode now enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Presidential Escort gamemode now disabled." };
                case "select":
                    {
                        if (args.Length <= 1) return new string[] { "Invalid player name." };

                        List<Player> players = plugin.Server.GetPlayers(args[1]);
                        Player player;

                        if (players == null || players.Count == 0) return new string[] { "Player not found!" };

                        player = players.OrderBy(ply => ply.Name.Length).First();

                        plugin.VIP = player;

                        return new string[] { player.Name + " selected as the next VIP!" };
                    }
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}
