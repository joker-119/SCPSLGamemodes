using Smod2.Commands;

namespace MassacreGamemode
{
    class MassacreCommand : ICommandHandler
    {
        private readonly Massacre plugin;

        public MassacreCommand(Massacre plugin) => this.plugin = plugin;

        public string GetCommandDescription() => "";

        public string GetUsage() =>
            "Massacre Enabled : " + plugin.Enabled + "\n" +
            "[Massacre / mascr / motdb] HELP \n" +
            "Massacre ENABLE \n" +
            "Massacre DISABLE";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

            switch (args[0].ToLower())
            {
                case "help":
                    return new string[]
                    {
                        "Massacre Command List",
                        "Massacre enable - Enables the Massacre gamemode.",
                        "Massacre disable - Disables the Massacre gamemode."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Massacre gamemode now enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Massacre gamemode now disabled." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}
