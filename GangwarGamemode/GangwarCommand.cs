using Smod2.Commands;


namespace Gangwar
{
    class GangwarCommand : ICommandHandler
    {
        private readonly Gangwar plugin;
        public GangwarCommand(Gangwar plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Gangwar Enabled : " + plugin.Enabled + "\n" +
                "[Gangwar / gang / gw] HELP \n" +
                "gang ENABLE \n" +
                "gang DISABLE \n";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

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
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Gangwar gamemode enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Gangwar gamemode disabled." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}