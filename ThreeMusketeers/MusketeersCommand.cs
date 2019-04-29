using Smod2.Commands;

namespace ThreeMusketeers
{
    internal class MusketeersCommand : ICommandHandler
    {
        private readonly Musketeers plugin;

        public MusketeersCommand(Musketeers plugin) => this.plugin = plugin;

        public string GetCommandDescription() => "";

        public string GetUsage() =>
            "Three Musketeers Enabled: " + plugin.Enabled + "\n" +
            "[3Musketeers / Musketeers / 3Musk] HELP \n" +
            "3musk ENABLE \n" +
            "3musk DISABLE \n";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

            switch (args[0].ToLower())
            {
                case "help":
                    return new string[]
                    {
                        "Musketeers Command List",
                        "Musketeers enable - Enables the Three Musketeers gamemode.",
                        "Musketeers disable - Disables the Three Musketeers gamemode."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();
                    return new string[] { "Three Musketeers gamemode enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();
                    return new string[] { "Three Musketeers gamemode disabled." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}