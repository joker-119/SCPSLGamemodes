using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using System.Collections.Generic;
using scp4aiur;

namespace PresidentialEscortGamemode
{
    [PluginDetails(
        author = "mkrzy",
        name = "Presidential Escort Gamemode",
        description = "Scientist (VIP) has to escape from SCPs with help of NTF",
        id = "mkrzy.gamemode.presidential",
        version = "1.6.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2
    )]
    public class PresidentialEscort : Plugin
    {
        internal static PresidentialEscort singleton;
        public static Player vip = null;
        public static bool vipEscaped = false;

        public static bool
            enabled = false,
            roundstarted = false;

        public static int
            vip_health,
            guard_health;

        public override void OnDisable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " has been disabled.");
        }

        public override void OnEnable()
        {
            singleton = this;
            this.Info(this.Details.name + " v." + this.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "presidentialescort", "presidential", "escort", "pe" }, new PresidentialEscortCommand());
            this.AddConfig(new ConfigSetting("vip_vip_health", 2500, SettingType.NUMERIC, true, "The amount of health VIP's start with."));
            this.AddConfig(new ConfigSetting("vip_guard_health", 200, SettingType.NUMERIC, true, "The amount of health guards have."));
            Timing.Init(this);
            new Functions(this);
        }
    }
}