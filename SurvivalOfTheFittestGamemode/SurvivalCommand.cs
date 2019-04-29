using Smod2.Commands;

namespace SurvivalGamemode
{
    internal class SurvivalCommand : ICommandHandler
    {
        private readonly Survival plugin;

        public SurvivalCommand(Survival plugin) => this.plugin = plugin;

        public string GetCommandDescription() => "";

        public string GetUsage() =>
            "Survival Enabled : " + plugin.Enabled + "\n" +
            "[Survival / suv / sotf] HELP \n" +
            "Survival ENABLE \n" +
            "Survival DISABLE";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

            switch (args[0].ToLower())
            {
                case "help":
                    return new string[]
                    {
                        "Survival Command List \n"+
                        "Survival enable - Enables the Survival gamemode. \n"+
                        "Survival disable - Disables the Survival gamemode. \n"
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Survival gamemode now enabled." };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Survival gamemode now disabled." };
                case "zone":
                    if (args.Length <= 1) return new string[] { "Invalid zone." };

                    switch (args[1].ToLower())
                    {
                        case "hcz":
                            plugin.Zone = "hcz";
                            return new string[] { "Heavy Containment Zone selected." };
                        case "lcz":
                            plugin.Zone = "lcz";
                            return new string[] { "Light Containment zone selected." };
                        default:
                            return new string[] { "A valid zone must be specified." };
                    }
                default:
                    return new string[]
                    {
                            GetUsage()
                    };
            }
        }
    }
}
