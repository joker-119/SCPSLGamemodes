using Smod2.Commands;

namespace MuskateersGamemode
{
    class MuskateersCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }
        public string GetUsage()
        {
            return "Three Muskateers Enabled: " + Muskateers.enabled + "\n" +
                    "[3Muskateers / Muskateers / 3Musk] HELP \n"+
                    "3musk ENABLE \n"+
                    "3musk DISABLE \n";
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
                            "Muskateers Command List \n"+
                            "Muskateers enable - Enables the Three Muskateers gamemode. \n"+
                            "Muskateers disable - Disables the Three Muskateers gamemode. \n"
                        };
                    case "enable":
                        Functions.singleton.EnableGamemode();
                        return new string[]
                        {
                            "Three Muskateers will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.singleton.DisableGamemode();
                        return new string[]
                        {
                            "Three Muskateers gamemode now disabled."
                        };
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