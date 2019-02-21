using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;

namespace MassacreGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Massacre of the D-Bois Gamemode",
        description = "Gamemode Template",
        id = "gamemode.massacre",
        version = "1.0",
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
            this.AddConfig(new ConfigSetting("mass_spawn_room", "jail", SettingType.BOOL, true, "Where everyone should spawn."));
        }

        public static void EnableGamemode()
        {
            enabled = true;
            if (!roundstarted)
            {
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Massacre of the D-Bois Gamemode</color> is starting..", false);
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
        public static Vector SpawnLoc()
        {
            Vector spawn = null;
            if (Massacre.SpawnRoom.ToLower() == "jail")
            {
                Massacre.plugin.Info("Jail room selected");
                spawn = new Vector(53,1020,-44);
            }
            else if (Massacre.SpawnRoom.ToLower() == "939")
            {
                Massacre.plugin.Info("939 Spawn Room selected");
                spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);
            }
            else if (Massacre.SpawnRoom.ToLower() == "049")
            {
                Massacre.plugin.Info("SCP-049 Spawn room selected");
                spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);
            }
            else if (Massacre.SpawnRoom.ToLower() == "173")
            {
                Massacre.plugin.Info("SCP-173 Spawn room selected.");
                spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_173);
            }
            else if (Massacre.SpawnRoom.ToLower() == "106")
            {
                Massacre.plugin.Info("SCP-106 Spawn room selected.");
                spawn = Massacre.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_106);
            }
            else
            {
                Massacre.plugin.Info("An invalid spawn room was entered, defaulting to Jail.");
                spawn = new Vector(53,1020,-44);
            }
            return spawn;
        }
    }
}