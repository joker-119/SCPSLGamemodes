using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using System.Collections.Generic;
using System.Linq;
using scp4aiur;

namespace SurvivalGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Survival of the Fittest Gamemode",
        description = "Gamemode Template",
        id = "gamemode.survival",
        version = "1.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2
    )]
    public class Survival : Plugin
    {
        internal static Survival plugin;
        
        public static bool
            enabled = false,
            roundstarted = false;

        public static float nut_delay;

        public static int nut_health;
        
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
            this.AddCommands(new string[] { "survival", "sotf", "surv" }, new SurvivalCommand());
            Timing.Init(this);
            this.AddConfig(new ConfigSetting("survival_peanut_delay", 120f, SettingType.FLOAT, true, "The amount of time to wait before unleading peanuts."));
            this.AddConfig(new ConfigSetting("survival_peanut_health", 173, SettingType.NUMERIC, true, "The amount of health peanuts should have (lower values move faster"));
        }

        public static void EnableGamemode()
        {
            enabled = true;
            if (!roundstarted)
            {
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Survival of the Fittest Gamemode</color> is starting..", false);
            }
        }
        public static void DisableGamemode()
        {
            enabled = false;
            plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
    }

    public class Functions
    {
        public static Vector NutSpawn()
        {
            List<Room> rooms = new List<Room>();
            foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
            {
                if (room.ZoneType == ZoneType.HCZ)
                {
                    rooms.Add(room);
                }
            }
            int randomNum = new System.Random().Next(rooms.Count());
            Room randomRoom = rooms[randomNum];
            Vector spawn = randomRoom.Position;
            return spawn;
        }
    }
}