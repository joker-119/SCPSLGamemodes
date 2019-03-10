using Smod2.Commands;

namespace LurkingGamemode
{
    class LurkingCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Lurking Enabled : " + Lurking.enabled + "\n"+
                "[Lurking / lurk / litd] HELP \n" +
                "Lurking ENABLE \n" +
                "Lurking DISABLE";
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
                            "Lurking Command List \n"+
                            "Lurking enable - Enables the Lurking gamemode. \n"+
                            "Lurking disable - Disables the Lurking gamemode. \n"
                        };
                    case "enable":
                        Functions.singleton.EnableGamemode();
                        return new string[]
                        {
                            "Lurking will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.singleton.DisableGamemode();
                        return new string[]
                        {
                            "Lurking gamemode now disabled."
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