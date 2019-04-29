using Smod2.Commands;

namespace Bomber
{
    internal class BomberCommand : ICommandHandler
    {
        private readonly Bomber plugin;
        public BomberCommand(Bomber plugin) => this.plugin = plugin;

        public string GetCommandDescription() => "";

        public string GetUsage() =>
            "[Bomberman / bomb] HELP \n" +
            "bomb drop" +
            "bomb flash" +
            "bomb select" +
            "bomb bosswave";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length <= 0) return new string[] { GetUsage() };
            if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };

            switch (args[0].ToLower())
            {
                case "help":
                    return new string[]
                    {
                        "Bomberman Command List",
                        "bomb enable - Enables the Bomberman gamemode.",
                        "bomb disable - Disables the Bomberman gamemode.",
                        "bomb drop - Repeats the previous grenade drop.",
                        "bomb select Class - Selects the specified class as the rounds spawn. Accepts [classd, sci, guard, ntf, chaos, normal]",
                        "bomb bosswave - Cuts the time for grenade spawns in half."
                    };
                case "drop":
                    {
                        plugin.Functions.GetPlayers();
                        plugin.Functions.DropGrenades();
                        return new string[] { "Dropping grenades on all players." };
                    }
                case "flash":
                    {
                        plugin.Functions.GetPlayers();
                        plugin.Functions.DropFlash();
                        return new string[] { "Dropping flash grenades on all players!" };
                    }
                case "bosswave":
                    {
                        plugin.Functions.BossWave();
                        return new string[] { "Grenades will now spawn twice as fast, hehe xd" };
                    }
                case "select":
                    {
                        if (args.Length <= 1) return new string[] { "A class must be specified." };
                        
                        switch (args[1].ToLower())
                        {
                            case "classd":
                                plugin.SpawnClass = "classd";
                                return new string[] { "Class-D spawn has been selected." };
                            case "sci":
                            case "nerd":
                            case "scientist":
                                plugin.SpawnClass = "sci";
                                return new string[] { "Scientist spawn has been selected." };
                            case "guard":
                                plugin.SpawnClass = "guard";
                                return new string[] { "Facility Guard spawn has been selected." };
                            case "ntf":
                                plugin.SpawnClass = "ntf";
                                return new string[] { "NTF Spawn has been selected." };
                            case "chaos":
                                plugin.SpawnClass = "chaos";
                                return new string[] { "Chaos Insurgency spawn has been selected." };
                            case "normal":
                                plugin.SpawnClass = "";
                                return new string[] { "Normal spawning has been selected." };
                            case "war":
                                plugin.SpawnClass = "war";
                                return new string[] { "The war has begun!" };
                            default:
                                return new string[] { "An invalid class was given!" };
                        }
                    }
                   default:
                       return new string[] { GetUsage() };
            }
        }
    }
}