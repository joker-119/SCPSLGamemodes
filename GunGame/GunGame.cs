using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using scp4aiur;
using System.Collections.Generic;
using System;

namespace Gungame
{
    [PluginDetails(
        author = "Joker119",
        name = "GunGame Gamemode",
        description = "Kill EVERYONE!",
        id = "gungame.gamemode",
        version = "1.6.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class GunGame : Plugin
    {
        internal static GunGame singleton;
        public static Random gen = new System.Random();
        public static List<Room> rooms = new List<Room>();
        public static bool
            enabled = false,
            roundstarted = false,
            reversed;
        public static int
            health;
        public static Player winner = null;
        public List<RoomType> validRooms = new List<RoomType>() { RoomType.CLASS_D_CELLS, RoomType.CAFE, RoomType.AIRLOCK_00, RoomType.AIRLOCK_01, RoomType.INTERCOM, RoomType.PC_LARGE, RoomType.PC_SMALL, RoomType.SCP_049, RoomType.SCP_096, RoomType.SCP_173, RoomType.SCP_372, RoomType.SCP_939 };

        public static string zone;

        public override void OnDisable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " has been diisabled.");
        }

        public override void OnEnable()
        {
            singleton = this;
            this.Info(this.Details.name + " v." + this.Details.version + " has been enabled.");
        }
        public override void Register()
        {
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { " gungame", "gun" }, new GunGameCommand());
            this.AddConfig(new ConfigSetting("gun_reversed", true, SettingType.BOOL, true, "If the traditional gungame mode should be reversed."));
            this.AddConfig(new ConfigSetting("gun_spawn_zone", "lcz", SettingType.STRING, true, "Where you should spawn."));
            this.AddConfig(new ConfigSetting("gun_health", 100, SettingType.NUMERIC, true, "How much healt you will have."));
            Timing.Init(this);
            new Functions(this);
        }
    }
}