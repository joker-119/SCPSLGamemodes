using Smod2.Commands;

namespace MuskateersGamemode
{
    class MuskateersCommand : ICommandHandler
    {
        private readonly Muskateers plugin;

        public MuskateersCommand(Muskateers plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Three Muskateers Enabled: " + plugin.Enabled + "\n" +
                    "[3Muskateers / Muskateers / 3Musk] HELP \n" +
                    "3musk ENABLE \n" +
                    "3musk DISABLE \n";
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
                        "Muskateers Command List",
                        "Muskateers enable - Enables the Three Muskateers gamemode.",
                        "Muskateers disable - Disables the Three Muskateers gamemode."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();
                    return new string[] { "Three Muskateers gamemode enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();
                    return new string[] { "Three Muskateers gamemode disabled." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}