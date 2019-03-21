using Smod2.Commands;


namespace ZombielandGamemode
{
    class ZombielandCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "zombieland Enabled : " + Zombieland.enabled + "\n" +
                "[zombieland / zombie / zl] HELP \n" +
                "zombieland ENABLE \n" +
                "zombieland DISABLE";
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
                            "Zombieland Command List \n"+
                            "Zombieland enable - Enables the Zombieland gamemode. \n"+
                            "Zombieland disable - Disables the Zombieland gamemode. \n"
                        };
                    case "enable":
                        Functions.singleton.EnableGamemode();
                        return new string[]
                        {
                            "Zombieland will be enabled for the next round!"
                        };
                    case "disable":
                        Functions.singleton.DisableGamemode();
                        return new string[]
                        {
                            "Zombieland gamemode now disabled."
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
