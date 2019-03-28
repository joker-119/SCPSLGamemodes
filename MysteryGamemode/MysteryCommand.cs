using System.Linq;
using Smod2.Commands;
using Smod2.API;
using System.Collections.Generic;

namespace Mystery
{
    class MysteryCommand : ICommandHandler
    {
        private readonly Mystery plugin;

        public MysteryCommand(Mystery plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Mystery Enabled : " + plugin.Enabled + "\n" +
                "[mystery / murder] HELP \n" +
                "mystery ENABLE \n" +
                "mystery DISABLE";
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
                            "Mystery Command List",
                            "mystery enable - Enables the Murder Mystery gamemode.",
                            "mystery disable - Disables the Murder Mystery gamemode."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Mystery gamemode enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Mystery gamemode disabled." };
                case "spawn":
                    if (plugin.Enabled && plugin.RoundStarted && args.Length > 2)
                    {
                        List<Player> players = plugin.Server.GetPlayers(args[1]);
                        Player player;

                        if (players == null || players.Count == 0) return new string[] { "Player not found." };

                        player = players.OrderBy(ply => ply.Name.Length).First();

                        if (player == null)
                        {
                            return new string[] { "Couldn't get player: " + args[1] };
                        }
                        switch (args[2].ToLower())
                        {
                            case "det":
                                {
                                    plugin.Functions.SpawnDet(player);

                                    return new string[] { "Player " + player.Name + " spawned as a Detective." };
                                }
                            case "murd":
                                {
                                    plugin.Functions.SpawnMurd(player);

                                    return new string[] { "Player " + player.Name + " spawned as a Murderer." };
                                }
                            case "civ":
                                {
                                    plugin.Functions.SpawnCiv(player);

                                    return new string[] { "Player " + player.Name + " spawned as a Civilian." };
                                }
                        }
                    }
                    return new string[] { "This command must specify a player and role, and can only be used after the round has begun." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}