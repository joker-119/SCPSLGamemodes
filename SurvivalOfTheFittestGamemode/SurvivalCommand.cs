using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (args[0].ToLower() == "help")
                {
                    return new string[] {
                        "Survival Command List \n" +
                        "Survival enable - Enable Survival for the next round. \n" +
                        "Survival disable - Disable Survival this and following rounds."
                    };
                }
                else if (args[0].ToLower() == "enable") { Survival.EnableGamemode(); return new string[] { "Survival will be enabled next round!" }; }
                else if (args[0].ToLower() == "disable") { Survival.DisableGamemode(); return new string[] { "Survival is now disabled." }; }
                else
                    return new string[] { GetUsage() };
            }
            else
                return new string[] { GetUsage() };
        }
    }
}
