using Smod2.Commands;

namespace SurvivalGamemode
{
    class SurvivalCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Survival Enabled : " + Survival.enabled + "\n"+
                "[Survival / suv / sotf] HELP \n" +
                "Survival ENABLE \n" +
                "Survival DISABLE";
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
                            "Survival Command List \n"+
                            "Survival enable - Enables the Survival gamemode. \n"+
                            "Survival disable - Disables the Survival gamemode. \n"
                        };
                    case "enable":
                        Functions.EnableGamemode();
                        return new string[]
                        {
                            "Survival will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.DisableGamemode();
                        return new string[]
                        {
                            "Survival gamemode now disabled."
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
