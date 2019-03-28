using Smod2.Commands;

namespace LurkingGamemode
{
    class LurkingCommand : ICommandHandler
    {
        private readonly Lurking plugin;

        public LurkingCommand(Lurking plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Lurking Enabled : " + plugin.Enabled + "\n" +
                "[Lurking / lurk / litd] HELP \n" +
                "Lurking ENABLE \n" +
                "Lurking DISABLE";
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
                        "Lurking Command List",
                        "Lurking enable - Enables the Lurking gamemode.",
                        "Lurking disable - Disables the Lurking gamemode."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Lurking gamemode enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Lurking gamemode disabled." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}