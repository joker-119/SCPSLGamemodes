using Smod2.Commands;


namespace ZombielandGamemode
{
    class ZombielandCommand : ICommandHandler
    {
        private readonly Zombieland plugin;

        public ZombielandCommand(Zombieland plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "zombieland Enabled : " + plugin.Enabled + "\n" +
                "[zombieland / zombie / zl] HELP \n" +
                "zombieland ENABLE \n" +
                "zombieland DISABLE";
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
                        "Zombieland Command List \n"+
                        "Zombieland enable - Enables the Zombieland gamemode. \n"+
                        "Zombieland disable - Disables the Zombieland gamemode. \n"
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();
                    return new string[] { "Zombieland gamemode now enavled." };
                case "disable":
                    plugin.Functions.DisableGamemode();
                    return new string[] { "Zombieland gamemode now disabled." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}
