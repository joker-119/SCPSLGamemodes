using Smod2.Commands;


namespace GangwarGamemode
{
    class GangwarCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Gangwar Enabled : " + Gangwar.enabled + "\n"+
                "[Gangwar / gang / gw] HELP \n" +
                "gang ENABLE \n" +
                "gang DISABLE \n";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "help")
                {
                    return new string[] {
                        "Gangwar Command List \n" +
                        "Gangwar enable - Enable Gangwar for the next round. \n" +
                        "Gangwar disable - Disable gangwar for following rounds. \n"
                    };
                }
                else if (args[0].ToLower() == "enable")
                {
                    Gangwar.EnableGamemode();
                    return new string[]
                    {
                        "Gangwar will be enabled next round!"
                    };
                }
                else if (args[0].ToLower() == "disable")
                {
                    Gangwar.DisableGamemode();
                    return new string[]
                    {
                        "Gangwar is now disabled."
                    };
                }
                else
                    return new string[] 
                    {
                        GetUsage()
                    };
            }
            else
                return new string[]
                {
                    GetUsage()
                };
        }
    }
}