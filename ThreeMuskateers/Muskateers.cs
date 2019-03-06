using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using System;

namespace MuskateersGamemode
{
    [PluginDetails(
        author = "Joker119",
        name = "Three Muskateers Gamemode",
        description = "3 NTF Vs. a crap load of Class-D",
        id = "muskateers.gamemode",
        version = "1.3.5",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class Muskateers : Plugin
    {
        internal static Muskateers plugin;
        public static bool
            enabled = false,
            roundstarted = false;
        public static int ntf_health;
        public static int classd_health;
        public static Random generator = new System.Random();
        
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
            this.AddCommands(new string[] { "3muskateers", "muskateers", "3musk" }, new MuskateersCommand());
            this.AddConfig(new ConfigSetting("musk_ntf_health", 4500, SettingType.NUMERIC, true, "How much Health NTF spawn with."));
            this.AddConfig(new ConfigSetting("musk_classd_health", 100, SettingType.NUMERIC, true, "How much health Class-D spawn with."));
        }
    }

    public class Functions
    {
        public static void EnableGamemode()
        {
            Muskateers.enabled = true;
            if (!Muskateers.roundstarted)
            {
                Muskateers.plugin.pluginManager.Server.Map.ClearBroadcasts();
                Muskateers.plugin.pluginManager.Server.Map.Broadcast(25, "<color=#308ADA> Three Muskateers</color> gamemode starting..", false);
            }
        }
        public static void DisableGamemode()
        {
            Muskateers.enabled = false;
            Muskateers.plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
        public static IEnumerable<float> SpawnNTF(Player player, float delay)
        {
            player.ChangeRole(Role.NTF_COMMANDER, true, true, true, true);
			yield return 2;
            player.SetHealth(Muskateers.ntf_health);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#308ADA>Muskateer</color>. Enter the facility and eliminate all Class-D.", false);
        }
        public static IEnumerable<float> SpawnClassD(Player player, float delay)
        {
            player.ChangeRole(Role.CLASSD, true, true, true, true);
			yield return 2;
            player.SetHealth(Muskateers.classd_health);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#DAA130>Class-D personnel</color>. Escape the facility before the auto-nuke, but evade the NTF sent to kill you!", false);
        }
        public static void EndGamemodeRound()
        {
            if (Muskateers.enabled)
            {
                Muskateers.plugin.Info("The Gamemode round has ended!");
                Muskateers.roundstarted = false;
                Muskateers.plugin.Server.Round.EndRound();
            }
        }
    }
}