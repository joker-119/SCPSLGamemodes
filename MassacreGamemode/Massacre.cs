using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using System.Collections.Generic;

namespace MassacreGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Massacre of the D-Bois Gamemode",
        description = "Gamemode Template",
        id = "gamemode.massacre",
        version = "1.3.5",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]
    public class Massacre : Plugin
    {
        internal static Massacre plugin;
        
        public static bool
            enabled = false,
            roundstarted = false;
        public static string SpawnRoom;
        public static Vector SpawnLoc;
        public static int nut_health;
        public static List<Vector> SpawnLocs = new List<Vector>();
        
        public override void OnDisable()
        {
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been disabled.");
        }

        public override void OnEnable()
        {
            plugin = this;
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "massacre", "motdb", "mascr" }, new MassacreCommand());
            this.AddConfig(new ConfigSetting("mass_spawn_room", "jail", SettingType.STRING, true, "Where everyone should spawn."));
            this.AddConfig(new ConfigSetting("mass_peanut_health", 1, SettingType.NUMERIC, true, "How much health Peanuts spawn with."));
        }
    }

    public class Functions
    {
        public static void EnableGamemode()
        {
            Massacre.enabled = true;
            if (!Massacre.roundstarted)
            {
                Massacre.plugin.pluginManager.Server.Map.ClearBroadcasts();
                Massacre.plugin.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Massacre of the D-Bois Gamemode</color> is starting..", false);
            }
        }
        public static void DisableGamemode()
        {
            Massacre.enabled = false;
            Massacre.plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
        public static Vector SpawnLoc()
        {
            Vector spawn = null;

            switch (Massacre.SpawnRoom.ToLower())
            {
                case "jail":
                {
                    Massacre.plugin.Info("Jail room selected.");
                    spawn = new Vector(53,1020,-44);
                    return spawn;
                }
                case "939":
                {
                    Massacre.plugin.Info("939 Spawn Room selected");
                    spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);
                    return spawn;
                }
                case "049":
                {
                    Massacre.plugin.Info("049 Spawn room selected");
                    spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);
                    return spawn;
                }
                case "106":
                {
                    Massacre.plugin.Info("106 Spawn room selected");
                    spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_106);
                    return spawn;
                }
                case "173":
                {
                    Massacre.plugin.Info("173 Spawn room selected.");
                    spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_173);
                    return spawn;
                }
                case "random":
                {
                    Massacre.SpawnLocs.Add(Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53));
                    Massacre.SpawnLocs.Add(Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_173));
                    Massacre.SpawnLocs.Add(Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049));
                    Massacre.SpawnLocs.Add(Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_106));
                    Massacre.SpawnLocs.Add(new Vector(53,1020,-44));
                    int RandomInt = new System.Random().Next(Massacre.SpawnLocs.Count);
                    return Massacre.SpawnLocs[RandomInt];
                }
                default:
                {
                    Massacre.plugin.Info("Invalid location selected, defaulting to Jail.");
                    spawn = new Vector(53,1020,-44);
                    return spawn;
                }
            }
        }
        public static void SpawnDboi(Player player)
        {
            player.ChangeRole(Role.CLASSD, false, false, false, true);
            player.Teleport(Massacre.SpawnLoc);

            foreach (Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.CUP);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Get ready to die!", false);
        }
        public static void SpawnNut(Player player)
        {
            player.ChangeRole(Role.SCP_173, false, true, true, true);
            player.Teleport(Massacre.SpawnLoc);
            Massacre.plugin.Info("Spawned " + player.Name + " as SCP-173");
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(35, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the D-bois!", false);
        }
        public static void EndGamemodeRound()
        {
            if (Massacre.enabled)
            {
                Massacre.plugin.Info("EndgameRound Function");
                Massacre.roundstarted = false;
                Massacre.plugin.Server.Round.EndRound();
            }
        }
    }
}