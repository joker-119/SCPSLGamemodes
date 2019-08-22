using Smod2.Commands;
using Smod2.API;

namespace HostageGamemode
{
    internal class Commands : ICommandHandler
    {
        private readonly HostageGamemode plugin;

        public Commands(HostageGamemode plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "GunGame Enabled : " + plugin.Enabled + "\n" +
            "[gungame / gun] HELP \n" +
            "gun ENABLE \n" +
            "gun DISABLE \n" +
            "gun SELECT [ZONETYPE]";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

            Player p = sender as Player;
            switch (args[0].ToLower())
            {
                case "help":
                    return new string[]
                    {
                        "Hostage Command List",
                        "hostage enable - Enables the gamemode.",
                        "hostage disable - Disables the gamemode."
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Hostage gamemode will be Enabled for the next round!" };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Hostage gamemode now disabled." };
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}