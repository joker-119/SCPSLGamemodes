using Smod2.Commands;
using Smod2.API;

namespace JuggernautGamemode
{
    class JuggernautCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Juggernaut Enabled : " + Juggernaut.enabled + "\n" +
                "[JUGGERNAUT / JUGG / JUG] HELP \n" +
                "JUGGERNAUT ENABLE \n" +
                "JUGGERNAUT DISABLE \n" +
                "JUGGERNAUT SELECT [PlayerName]";
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
                        "Juggernaut Command List \n"+
                        "Juggernaut enable - Enables the Juggernaut gamemode. \n"+
                        "Juggernaut disable - Disables the Juggernaut gamemode. \n"+
                        "Juggernaut select [PlayerName] - Selects the player to be the Juggernaut"
                        };
                    case "enable":
                        Functions.singleton.EnableGamemode();
                        return new string[]
                        {
                            "Juggernaut will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.singleton.DisableGamemode();
                        return new string[]
                        {
                            "Juggernaut will be disabled for the next round!"
                        };
                    case "select":
                        if (Juggernaut.enabled && args.Length > 1)
                        {
                            Player player = GetPlayerFromString.GetPlayer(args[1]);
                            if (player == null)
                            {
                                return new string[] { " Couldn't get player: " + args[1] };
                            }
                            Juggernaut.selectedJuggernaut = player;
                            Juggernaut.singleton.Info("" + player.Name + " chosen as the Juggernaut!");
                            return new string[] { " Player " + player.Name + " selected as the next Juggernaut!" };
                        }
                        return new string[] { "A player name must be specified, and the gamemode must be enabled!" };
                    default:
                        return new string[]
                        {
                            GetUsage()
                        };
                }
            }
            else
            {
                return new string[]
                {
                    GetUsage()
                };
            }
        }
    }
}
