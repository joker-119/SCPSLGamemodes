using Smod2.Commands;


namespace Gangwar
{
    class GangwarCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Gangwar Enabled : " + Gangwar.enabled + "\n" +
                "[Gangwar / gang / gw] HELP \n" +
                "gang ENABLE \n" +
                "gang DISABLE \n";
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
                            "Gangwar Command List \n"+
                            "Gangwar enable - Enables the Gangwar gamemode. \n"+
                            "Gangwar disable - Disables the Gangwar gamemode. \n"
                        };
                    case "enable":
                        Functions.singleton.EnableGamemode();
                        return new string[]
                        {
                            "Gangwar will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.singleton.DisableGamemode();
                        return new string[]
                        {
                            "Gangwar gamemode now disabled."
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