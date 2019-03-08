using Smod2;
using Smod2.Events;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.API;
using UnityEngine;
using System.Collections.Generic;

namespace JuggernautGamemode
{
    [PluginDetails(
        author = "Mozeman",
        name = "Juggernaut Gamemode",
        description = "Gamemode Template",
        id = "gamemode.juggernaut",
        version = "1.4.0",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
        )]
    public class Juggernaut : Plugin
    {
        internal static Juggernaut plugin;
        public static bool
            NTF_Disarmer,
            jugg_infinite_nades;
        public static int
            Jugg_base,
            Jugg_increase,
            Jugg_grenade,
            NTF_ammo,
            NTF_Health,
            juggernaut_health,
            ntf_health;
        public static Player
            juggernaut = null,
            activator = null,
            selectedJuggernaut = null,
            jugg_killer = null;
        public static float critical_damage;
        public static string[] juggernaut_prevRank = new string[2];
        public static HealthBar health_bar_type;



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
            this.AddConfig(new ConfigSetting("juggernaut_ntf_disarmer", false, SettingType.BOOL, true, "Wether or not NTF should spawn with Disarmers."));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_ammo", 272, SettingType.NUMERIC, true, "The amount of ammo NTF Commanders should spawn with."));
            this.AddConfig(new ConfigSetting("juggernaut_ntf_health", 150, SettingType.NUMERIC, true, "The amount of health the first wave of NTF should have."));
            this.AddConfig(new ConfigSetting("juggernaut_critical_damage", (float)0.15, SettingType.FLOAT, true, "The amount of critical damage the Juggernaut should recieve."));
            this.AddConfig(new ConfigSetting("juggernaut_infinite_jugg_nades", false, SettingType.BOOL, true, "If the Juggernaut should have infinite grenades."));
            this.AddConfig(new ConfigSetting("juggernaut_health_bar_type", "bar", false, SettingType.STRING, true, "Type of Health Bar to use for Juggernaut"));
        }
    }

    public class Functions
    {
        public static void EnableGamemode()
        {
            Juggernaut.enabled = true;
            if (!Juggernaut.roundstarted)
            {
                Juggernaut.plugin.pluginManager.Server.Map.ClearBroadcasts();
                Juggernaut.plugin.pluginManager.Server.Map.Broadcast(25, "<color=#228B22>Juggernaut Gamemode</color> is starting...", false);
            }
        }
        public static void DisableGamemode()
        {
            Juggernaut.enabled = false;
            Juggernaut.plugin.pluginManager.Server.Map.ClearBroadcasts();
        }
        public static bool IsJuggernaut(Player player)
        {
            if (Juggernaut.juggernaut != null)
            {
                if (player.Name == Juggernaut.juggernaut.Name || player.SteamId == Juggernaut.juggernaut.SteamId)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static Player GetJuggernautPlayer()
        {
            foreach (Player player in Juggernaut.plugin.pluginManager.Server.GetPlayers())
            {
                if (IsJuggernaut(player))
                {
                    return player;
                }
                else
                {
                    Juggernaut.plugin.Warn("Juggernaut not found!");
                    //ResetJuggernaut();
                }
            }
            return null;
        }

        public static Vector GetRandomPDExit()
        {
            List<Vector3> list = new List<Vector3>();
            GameObject[] exits_array = GameObject.FindGameObjectsWithTag("RoomID");
            foreach (GameObject exit in exits_array)
            {
                if (exit.GetComponent<Rid>() != null)
                    list.Add(exit.transform.position);
            }

            Vector3 chosenExit = list[UnityEngine.Random.Range(0, list.Count)];

            Vector SmodExit = new Vector(chosenExit.x, chosenExit.y += 2f, chosenExit.z);
            return SmodExit;
        }

        public static void CriticalHitJuggernaut(Player player)
        {
            //Vector position = PluginManager.Manager.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
            Vector position = GetRandomPDExit();
            int damage = (int)(Juggernaut.juggernaut_health * Juggernaut.critical_damage);
            player.Damage(damage, DamageType.FRAG);
            player.Teleport(position);
            Juggernaut.plugin.pluginManager.Server.Map.Broadcast(10, "The <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
            Juggernaut.plugin.Debug("Juggernaut Disarmed & Teleported");
        }

        public static void CriticalHitJuggernaut(Player player, Player activator)
        {
            //Vector position = PluginManager.Manager.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
            Vector position = GetRandomPDExit();
            int damage = (int)(Juggernaut.juggernaut_health * Juggernaut.critical_damage);
            player.Damage(damage, DamageType.FRAG);
            player.Teleport(position);
            Juggernaut.plugin.pluginManager.Server.Map.Broadcast(10, "" + activator.Name + " has sacrifieced themselves and made the <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
            Juggernaut.plugin.Debug("Juggernaut Disarmed & Teleported");
        }

        public static void ResetJuggernaut(Player player)
        {
            if (Juggernaut.juggernaut_prevRank != null && Juggernaut.juggernaut_prevRank.Length == 2)
                player.SetRank(Juggernaut.juggernaut_prevRank[0], Juggernaut.juggernaut_prevRank[1]);
            else
                Juggernaut.juggernaut.SetRank();
            ResetJuggernaut();
        }

        public static void ResetJuggernaut()
        {
            Juggernaut.plugin.Info("Resetting Juggernaut.");
            Juggernaut.juggernaut = null;
            Juggernaut.juggernaut_prevRank = null;
            Juggernaut.juggernaut_health = 0;
        }

        public static void EndGamemodeRound()
        {
            Juggernaut.plugin.Info("EndgameRound Function");
            ResetJuggernaut();
            Juggernaut.roundstarted = false;
            Juggernaut.plugin.Server.Round.EndRound();
        }

        public static IEnumerable<float> SpawnAsNTFCommander(Player player)
        {
            player.ChangeRole(Role.NTF_COMMANDER, false, true, true, true);
			yield return 1;

            Juggernaut.ntf_health = Juggernaut.NTF_Health;
            Juggernaut.plugin.Info("SpawnNTF Health");
            player.SetHealth(Juggernaut.ntf_health);
			player.SetAmmo(AmmoType.DROPPED_5, 500);
			player.SetAmmo(AmmoType.DROPPED_7, 500);
			player.SetAmmo(AmmoType.DROPPED_9, 500);
			player.GiveItem(ItemType.FLASHLIGHT);

            player.PersonalClearBroadcasts();
            if (Juggernaut.juggernaut != null)
                player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut " + Juggernaut.juggernaut.Name + "</color>", false);
            else
                player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut</color>", false);
        }

        public static void SpawnAsJuggernaut(Player player)
        {
            Juggernaut.juggernaut = player;

            //Spawned as Juggernaut in 939s spawn location
            Vector spawn = Juggernaut.plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);
            player.ChangeRole(Role.CHAOS_INSURGENCY, false, false, true, true);
            player.Teleport(spawn);

            Juggernaut.juggernaut_prevRank = new string[] { player.GetUserGroup().Color, player.GetUserGroup().Name };

            // Given a Juggernaut badge
            player.SetRank("silver", "Juggernaut");

            // Health scales with amount of players in round
            int health = Juggernaut.Jugg_base + (Juggernaut.Jugg_increase * Juggernaut.plugin.Server.NumPlayers) - 500;
            player.SetHealth(health);
            Juggernaut.juggernaut_health = health;

            // Clear Inventory
            foreach (Smod2.API.Item item in player.GetInventory())
                item.Remove();

            //Increased Ammo
            player.SetAmmo(AmmoType.DROPPED_7, 2000);
            player.SetAmmo(AmmoType.DROPPED_5, 2000);
            player.SetAmmo(AmmoType.DROPPED_9, 2000);

            // 1 Logicer
            player.GiveItem(ItemType.LOGICER);

            // 1 O5 Keycard
            player.GiveItem(ItemType.O5_LEVEL_KEYCARD);

            // Frag Grenades
            for (int i = 0; i < Juggernaut.Jugg_grenade; i++)
            {
                player.GiveItem(ItemType.FRAG_GRENADE);
            }

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are the <color=#228B22>Juggernaut</color> Eliminate all <color=#002DB3>NTF Commanders</color>", false);
        }

        public static string DrawHealthBar(double percentage)
        {
            string bar = "<color=#fffff>(</color>";
            const int BAR_SIZE = 20;
            percentage *= 100;
            if (percentage == 0) return "(      )";
            for (double i = 0; i < 100; i += (100 / BAR_SIZE))
            {
                if (i < percentage)
                {
                    bar += "█";
                }
                else
                {
                    bar += "<color=#474747>█</color>";
                }
            }
            bar += "<color=#ffffff>)</color>";
            return bar;
        }
    }

    public enum HealthBar
    {
        Raw,
        Percentage,
        Bar
    }
}