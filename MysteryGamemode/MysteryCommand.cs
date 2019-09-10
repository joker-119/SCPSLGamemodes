using System.Collections.Generic;
using System.Linq;
using Smod2.API;
using Smod2.Commands;
using MEC;

namespace Mystery
{
    internal class MysteryCommand : ICommandHandler
    {
        private readonly Mystery plugin;

        public MysteryCommand(Mystery plugin) => this.plugin = plugin;

        public string GetCommandDescription() => "";

        public string GetUsage() => "[mystery / murder] HELP \n" + "mystery spawn \n";
        
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
                            "mystery spawn <player> <role> - Spawns the player as the selected role."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Murder Mystery gamemode will be Enabled for the next round!" };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Murder Mystery gamemode now disabled." };
                case "spawn":
                    if (!plugin.RoundStarted || args.Length <= 2) 
                        return new string[] { "This command must specify a player and role, and can only be used after the round has begun." };
                    
                    List<Player> players = plugin.Server.GetPlayers(args[1]);
                    Player player;

                    if (players == null || players.Count == 0) return new string[] { "Player not found." };

                    player = players.OrderBy(ply => ply.Name.Length).First();

                    if (player == null) return new string[] { "Couldn't get player: " + args[1] };
                    
                    switch (args[2].ToLower())
                    {
                        case "det":
                        {
                            Timing.RunCoroutine(plugin.Functions.SpawnDet(player));

                            return new string[] { "Player " + player.Name + " spawned as a Detective." };
                        }
                        case "murd":
                        {
                            Timing.RunCoroutine(plugin.Functions.SpawnMurd(player));

                            return new string[] { "Player " + player.Name + " spawned as a Murderer." };
                        }
                        case "civ":
                        {
                            Timing.RunCoroutine(plugin.Functions.SpawnCiv(player));

                            return new string[] { "Player " + player.Name + " spawned as a Civilian." };
                        }
                        default:
                            return new string[] { "Invalid role selected." };
                    }
                    default:
                        return new string[] { GetUsage() };
            }
        }
    }
}