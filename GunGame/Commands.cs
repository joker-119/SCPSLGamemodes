using Smod2.API;
using Smod2.Commands;

namespace Gungame
{
    internal class GunGameCommand : ICommandHandler
    {
        private readonly GunGame plugin;

        public GunGameCommand(GunGame plugin) => this.plugin = plugin;

        public string GetCommandDescription() => "";

        public string GetUsage() =>
            "[gungame / gun] HELP \n" +
            "gun SELECT [ZONETYPE]";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

            Player p = (Player)sender;
            
            switch (args[0].ToLower())
            {
                case "help":
                    return new string[]
                    {
                        "GunGame Command List",
                        "gun enable - Enables the gamemode.",
                        "gun disable - Disables the gamemode.",
                        "gun select [zonetype] - selects a specific zone for this round. Accepts lcz, hcz and ent"
                    };
                case "enable":
                    plugin.Functions.EnableGamemode();

                    return new string[] { "Gungame gamemode will be Enabled for the next round!" };
                case "disable":
                    plugin.Functions.DisableGamemode();

                    return new string[] { "Gungame gamemode now disabled." };
                case "debug":
                    if (!plugin.RoundStarted) return new string[] { "Please wait for the gungame round to begin before using this command." };
                    plugin.Functions.ReplaceGun(p);

                    return new string[] { "Debugging gun replacement for " + p.Name + "." };
                case "win":
                    if (!plugin.RoundStarted) return new string[] { "Please wait for the round to begin before using this command." };
                    plugin.Functions.AnnounceWinner(p);

                    return new string[] { p.Name + " is the winner!" };
                case "select":
                    if (args.Length <= 1) return new string[] { "You must specify a zone!" };
                    if (plugin.RoundStarted) return new string[] { "This command cannot be used once the round has started." };

                    switch (args[1].ToLower())
                    {
                        case "lcz":
                            plugin.Zone = "lcz";
                            return new string[] { "Light Containment Zone selected!" };
                        case "hcz":
                            plugin.Zone = "hcz";
                            return new string[] { "Heavy Containment Zone Selected!" };
                        case "ent":
                            plugin.Zone = "ent";
                            return new string[] { "Entrance zone selected!" };
                        default:
                            return new string[] { "An invalid zone was specified!" };
                    }
                default:
                    return new string[] { GetUsage() };
            }
        }
    }
}