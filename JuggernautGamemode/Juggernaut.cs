using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.Attributes;

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
        }
    }
}