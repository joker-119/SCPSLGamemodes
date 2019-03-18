using Smod2.Commands;

namespace PresidentialEscortGamemode
{
    class PresidentialEscortCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Presidential Escort Enabled : " + PresidentialEscort.enabled + "\n"+
                "[presidentialescort / presidential / escort / pe] HELP \n" +
                "presidential ENABLE \n" +
                "presidential DISABLE";
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
                            "Presidential Escort Command List \n"+
                            "Presidential Escort enable - Enables the Presidential Escort gamemode. \n"+
                            "Presidential Escort disable - Disables the Presidential Escort gamemode. \n"
                        };
                    case "enable":
                        Functions.singleton.EnableGamemode();
                        return new string[]
                        {
                            "Presidential Escort will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.singleton.DisableGamemode();
                        return new string[]
                        {
                            "Presidential Escort gamemode now disabled."
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
