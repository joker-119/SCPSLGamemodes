using Smod2.Commands;

namespace Bomber
{
    class BomberCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }
        public string GetUsage()
        {
            return "Bomberman Enabled : " + Bomber.enabled + "\n"+
                "[Bomberman / bomb] HELP \n"+
                "bomb enable \n"+
                "bomb disable \n"+
                "bomb drop"+
                "bomb flash"+
                "bomb select"+
                "bomb bosswave";
        }
        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length > 0)
            {
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
                            "bomb bosswave - Cuts the time for grenade spawns in half.",
                        };
                    case "enable":
                    {
                        Functions.singleton.EnableGamemode();
                        return new string[] {"Bomberman will be enabled for the next round!"};
                    }
                    case "disable":
                    {
                        Functions.singleton.DisableGamemode();
                        return new string[] {"Bomberman now disabled."};
                    }
                    case "drop":
                    {
                        Functions.singleton.GetPlayers();
                        Functions.singleton.DropGrenades();
                        return new string[] { "Dropping grenades on all players." };
                    }
                    case "flash":
                    {
                        Functions.singleton.GetPlayers();
                        Functions.singleton.DropFlash();
                        return new string[] { "Dropping flash grenades on all players!" };
                    }
                    case "bosswave":
                    {
                        Functions.singleton.BossWave();
                        return new string[] { "Grenades will now spawn twice as fast, hehe xd" };
                    }
                    case "select":
                    {
                        if (args.Length > 1)
                        {
                            switch (args[1].ToLower())
                            {
                                case "classd":
                                    Bomber.spawn_class = "classd";
                                    return new string[] {"Class-D spawn has been selected."};
                                case "sci":
                                case "nerd":
                                case "scientist":
                                    Bomber.spawn_class = "sci";
                                    return new string[] {"Scientist spawn has been selected."};
                                case "guard":
                                    Bomber.spawn_class = "guard";
                                    return new string[] {"Facility Guard spawn has been selected."};
                                case "ntf":
                                    Bomber.spawn_class = "ntf";
                                    return new string[] {"NTF Spawn has been selected."};
                                case "chaos":
                                    Bomber.spawn_class = "chaos";
                                    return new string[] {"Chaos Insurgency spawn has been selected."};
                                case "normal":
                                    Bomber.spawn_class = "";
                                    return new string[] {"Normal spawning has been selected."};
                                case "war":
                                    Bomber.spawn_class = "war";
                                    return new string[] {"The war has begun!"};
                                default:
                                    return new string[] {"An invalid class was given!"};
                            }
                        }
                        return new string[] {"A class must be specified."};
                    }
                    default:
                        return new string[] {GetUsage()};
                }
            }
            return new string[] {GetUsage()};
        }
    }
}