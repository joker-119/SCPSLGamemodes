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
            return "zombieland Enabled : " + Zombieland.enabled + "\n"+
                "[zombieland / zombie / zl] HELP \n" +
                "zombieland ENABLE \n" +
                "zombieland DISABLE";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "help")
                {
                    return new string[] {
                        "zombieland Command List \n" +
                        "zombieland enable - Enable zombieland for the next round. \n" +
                        "zombieland disable - Disable zombieland this and following rounds."
                    };
                }
                else if (args[0].ToLower() == "enable") { Zombieland.EnableGamemode(); return new string[] { "Zombieland will be enabled next round!" }; }
                else if (args[0].ToLower() == "disable") { Zombieland.DisableGamemode(); return new string[] { "Zombieland is now disabled." }; }
                else
                    return new string[] { GetUsage() };
            }
            else
                return new string[] { GetUsage() };
        }
    }
}
