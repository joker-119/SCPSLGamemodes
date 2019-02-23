using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;

namespace GangwarGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Gangwar Gamemode",
        description = "Gangwar Gamemode",
        id = "gamemode.gangwar",
        version = "1.3.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class Gangwar : Plugin
    {
        internal static Gangwar plugin;
        public static bool 
            enabled = false,
            roundstarted = false;

        public static int ci_health;
        public static int ntf_health;

        public override void OnDisable()
        {
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been diisabled.");
        }

        public override void OnEnable()
        {
            plugin = this;
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "gangwar", "gang", "gw" }, new GangwarCommand());
            this.AddConfig(new ConfigSetting("gangwar_ci_health", 120, SettingType.NUMERIC, true, "The amount of health CI have."));
            this.AddConfig(new ConfigSetting("gangwar_ntf_health", 150, SettingType.NUMERIC, true, "The amount of health NTF have."));
        }
    }

    public class Functions
    {
        public static void EnableGamemode()
        {
            Gangwar.enabled = true;
            if (!Gangwar.roundstarted)
            {
                Gangwar.plugin.pluginManager.Server.Map.ClearBroadcasts();
                Gangwar.plugin.pluginManager.Server.Map.Broadcast(25, "<color=#00ffff> Gangwar Gamemode is starting..</color>", false);
            }
        }

        public static void DisableGamemode()
        {
            Gangwar.enabled = false;
            Gangwar.plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
        public static void EndGamemodeRound()
        {
            if (Gangwar.enabled)
            {
                Gangwar.plugin.Info("EndgameRound Function.");
                Gangwar.roundstarted = false;
                Gangwar.plugin.Server.Round.EndRound();
            }
        }

        public static void SpawnChaos(Player player)
        {
            player.ChangeRole(Role.CHAOS_INSURGENCY, true, true, false, true);
        }

        public static void SpawnNTF(Player player)
        {
            player.ChangeRole(Role.NTF_COMMANDER, true, true, false, true);
            player.SetAmmo(AmmoType.DROPPED_5,500);
            player.SetAmmo(AmmoType.DROPPED_7,500);
            player.SetAmmo(AmmoType.DROPPED_9,500);
        }
    }
}