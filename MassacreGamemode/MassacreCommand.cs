using Smod2.Commands;

namespace MassacreGamemode
{
    class MassacreCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Massacre Enabled : " + Massacre.enabled + "\n"+
                "[Massacre / mascr / motdb] HELP \n" +
                "Massacre ENABLE \n" +
                "Massacre DISABLE";
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
                            "Massacre Command List \n"+
                            "Massacre enable - Enables the Massacre gamemode. \n"+
                            "Massacre disable - Disables the Massacre gamemode. \n"
                        };
                    case "enable":
                        Functions.singleton.EnableGamemode();
                        return new string[]
                        {
                            "Massacre will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.singleton.DisableGamemode();
                        return new string[]
                        {
                            "Massacre gamemode now disabled."
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
