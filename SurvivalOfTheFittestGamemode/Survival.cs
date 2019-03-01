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
        version = "1.3.5",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]
    public class Survival : Plugin
    {
        internal static Survival plugin;
        public static bool
            enabled = false,
            roundstarted = false;
        public static float nut_delay;
        public static Vector pos;
        public static int nut_health;
        public static bool blackouts;
        
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
    }

    public class Functions
    {
        public static void EnableGamemode()
        {
            Survival.enabled = true;
            if (!Survival.roundstarted)
            {
                Survival.plugin.pluginManager.Server.Map.ClearBroadcasts();
                Survival.plugin.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Survival of the Fittest Gamemode</color> is starting..", false);
            }
        }
        public static void DisableGamemode()
        {
            Survival.enabled = false;
            Survival.plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
        public static void EndGamemodeRound()
        {
            if (Survival.enabled)
            {
                Survival.plugin.Info("EndgameRound Function");
                Survival.roundstarted = false;
                Survival.plugin.Server.Round.EndRound();

                Survival.plugin.Info("Toggling Blackout off.");
                SCP575.Functions.singleton.ToggleBlackout();
                if (Survival.blackouts)
                {
                    Survival.plugin.Info("Enabling timed Blackouts.");
                    SCP575.Functions.singleton.EnableBlackouts();
                }
            }
        }

        public static void SpawnDboi(Player player)
        {
            Vector spawn = Survival.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_096);
            player.ChangeRole(Role.CLASSD, false, false, false, true);
            player.Teleport(spawn);

            foreach (Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.CUP);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Find a hiding place and survive from the peanuts! They will spawn in 939's area when the lights go off!", false);
        }

        public static void SpawnNut(Player player)
        {

            player.ChangeRole(Role.SCP_173, false, true, true, true);
            Survival.plugin.Info("Spawned " + player.Name + " as SCP-173");
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(35, "You will be teleported into the game arena when adequate time has passed for other players to hide...", false);
        }
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
        public static IEnumerable<float> TeleportNuts(float delay)
        {
            yield return delay;
            Survival.plugin.Info("Timer completed!");
            SCP575.Functions.singleton.ToggleBlackout();
            foreach (Player player in Survival.plugin.Server.GetPlayers())
            {
                if (player.TeamRole.Role == Role.SCP_173)
                {
                    player.Teleport(Survival.pos);
                    player.SetHealth(Survival.nut_health);
                    player.PersonalBroadcast(15, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the Class-D before the auto-nuke goes off!", false);
                }
            }
        }
    }
}