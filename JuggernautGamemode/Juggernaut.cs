using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;

namespace JuggernautGamemode
{
    [PluginDetails(
        author = "Mozeman",
        name = "Juggernaut Gamemode",
        description = "Gamemode Template",
        id = "gamemode.juggernaut",
        version = "1.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2
        )]
    public class Juggernaut : Plugin
    {
        internal static Juggernaut plugin;
        public static bool NTF_Disarmer;
        public static int Jugg_base;
        public static int Jugg_increase;
        public static int Jugg_grenade;
        public static int NTF_ammo;
        public static int NTF_Health;
        public static bool jugg_infinite_nades;


        public static bool
            enabled = false,
            roundstarted = false;

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
            // Register Events
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "jug", "jugg", "juggernaut" }, new JuggernautCommand());

            // Register Configs
            this.AddConfig(new ConfigSetting("juggernaut_base_health", 500, SettingType.NUMERIC, true, "The amoutn of base health the Juggernaut starts with."));
            this.AddConfig(new ConfigSetting("juggernaut_increase_amount", 500, SettingType.NUMERIC, true, "The amount of extra HP a Jugg gets for each additional player."));
            this.AddConfig(new ConfigSetting("juggernaut_jugg_grenades", 6, SettingType.NUMERIC, true, "The number of grenades the Jugg should start with."));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_disarmer", false, SettingType.BOOL, true, "Wether or not NTF should spawn with Disarmers." ));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_ammo", 272, SettingType.NUMERIC, true, "The amount of ammo NTF Commanders should spawn with."));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_health", 150, SettingType.NUMERIC, true, "The amount of health the first wave of NTF should have."));
            this.AddConfig(new ConfigSetting("juggernaut_critical_damage", (float)0.15, SettingType.FLOAT, true, "The amount of critical damage the Juggernaut should recieve."));
            this.AddConfig(new ConfigSetting("juggernaut_infinit_jugg_nades", false, SettingType.BOOL, true, "If the Juggernaut should have infinite grenades."));
        }

        public static void EnableGamemode()
        {
            enabled = true;
            if (!roundstarted)
            {
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.pluginManager.Server.Map.Broadcast(25, "<color=#228B22>Juggernaut Gamemode</color> is starting...", false);
            }
        }

        public static void DisableGamemode()
        {
            enabled = false;
            plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
    }
}