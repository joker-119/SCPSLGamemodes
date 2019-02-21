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
                if (args[0].ToLower() == "help")
                {
                    return new string[] {
                        "Massacre Command List \n" +
                        "Massacre enable - Enable Massacre for the next round. \n" +
                        "Massacre disable - Disable Massacre this and following rounds."
                    };
                }
                else if (args[0].ToLower() == "enable") { Massacre.EnableGamemode(); return new string[] { "Massacre will be enabled next round!" }; }
                else if (args[0].ToLower() == "disable") { Massacre.DisableGamemode(); return new string[] { "Massacre is now disabled." }; }
                else
                    return new string[] { GetUsage() };
            }
            else
                return new string[] { GetUsage() };
        }
    }
}
