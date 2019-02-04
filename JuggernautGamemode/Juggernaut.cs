using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
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
        public static int Jugg_ammo;
        public static int Jugg_grenade;
        public static int NTF_ammo;
        public static int NTF_Health;


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

            this.AddConfig(new ConfigSetting("Jugg_base_hp", 500, SettingType.NUMERIC, true, "The amoutn of base health the Juggernaut starts with."));
            this.AddConfig(new ConfigSetting("Jugg_increase_amount", 500, SettingType.NUMERIC, true, "The amount of extra HP a Jugg gets for each additional player."));
            this.AddConfig(new ConfigSetting("Jugg_ammo", 10000, SettingType.NUMERIC, true, "The amount of Logicer ammo the Jugg should start with."));
            this.AddConfig(new ConfigSetting("Jugg_grenades", 6, SettingType.NUMERIC, true, "The number of grenades the Jugg should start with."));
            this.AddConfig(new ConfigSetting("NTF_Disarmer", false, SettingType.BOOL, true, "Wether or not NTF should spawn with Disarmers." ));
            this.AddConfig(new ConfigSetting("NTF_ammo", 272, SettingType.NUMERIC, true, "The amount of ammo NTF Commanders should spawn with."));
            this.AddConfig(new ConfigSetting("NTF_Health", 150, SettingType.NUMERIC, true, "The amount of health the first wave of NTF should have."));
            
            //GamemodeManager.GamemodeManager.RegisterMode(this, "21111111111111111111");
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