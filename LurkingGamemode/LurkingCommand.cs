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
                if (args[0].ToLower() == "help")
                {
                    return new string[] {
                        "Lurking Command List \n" +
                        "Lurking enable - Enable Lurking for the next round. \n" +
                        "Lurking disable - Disable Lurking this and following rounds."
                    };
                }
                else if (args[0].ToLower() == "enable") { Lurking.EnableGamemode(); return new string[] { "Lurking will be enabled next round!" }; }
                else if (args[0].ToLower() == "disable") { Lurking.DisableGamemode(); return new string[] { "Lurking is now disabled." }; }
                else
                    return new string[] { GetUsage() };
            }
            else
                return new string[] { GetUsage() };
        }
    }
}