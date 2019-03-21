using Smod2;
using Smod2.API;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JuggernautGamemode
{
    public class Functions
    {
        public static Functions singleton;
        public Juggernaut Juggernaut;
        public Functions(Juggernaut plugin)
        {
            this.Juggernaut = plugin;
            Functions.singleton = this;
        }
        public void EnableGamemode()
        {
            Juggernaut.enabled = true;
            if (!Juggernaut.roundstarted)
            {
                Juggernaut.pluginManager.Server.Map.ClearBroadcasts();
                Juggernaut.pluginManager.Server.Map.Broadcast(25, "<color=#228B22>Juggernaut Gamemode</color> is starting...", false);
            }
        }
        public void DisableGamemode()
        {
            Juggernaut.enabled = false;
            Juggernaut.pluginManager.Server.Map.ClearBroadcasts();
        }
        public bool IsJuggernaut(Player player)
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

        public Player GetJuggernautPlayer()
        {
            foreach (Player player in Juggernaut.pluginManager.Server.GetPlayers())
            {
                if (IsJuggernaut(player))
                {
                    return player;
                }
                else
                {
                    Juggernaut.Warn("Juggernaut not found!");
                    //ResetJuggernaut();
                }
            }
            return null;
        }

        public Vector GetRandomPDExit()
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

        public void CriticalHitJuggernaut(Player player)
        {
            //Vector position = PluginManager.Manager.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
            Vector position = GetRandomPDExit();
            int damage = (int)(Juggernaut.juggernaut_health * Juggernaut.critical_damage);
            player.Damage(damage, DamageType.FRAG);
            player.Teleport(position);
            Juggernaut.pluginManager.Server.Map.Broadcast(10, "The <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
            Juggernaut.Debug("Juggernaut Disarmed & Teleported");
        }

        public void CriticalHitJuggernaut(Player player, Player activator)
        {
            //Vector position = PluginManager.Manager.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
            Vector position = GetRandomPDExit();
            int damage = (int)(Juggernaut.juggernaut_health * Juggernaut.critical_damage);
            player.Damage(damage, DamageType.FRAG);
            player.Teleport(position);
            Juggernaut.pluginManager.Server.Map.Broadcast(10, "" + activator.Name + " has sacrifieced themselves and made the <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
            Juggernaut.Debug("Juggernaut Disarmed & Teleported");
        }

        public void ResetJuggernaut(Player player)
        {
            if (Juggernaut.juggernaut_prevRank != null && Juggernaut.juggernaut_prevRank.Length == 2)
                player.SetRank(Juggernaut.juggernaut_prevRank[0], Juggernaut.juggernaut_prevRank[1]);
            else
                Juggernaut.juggernaut.SetRank();
            ResetJuggernaut();
        }

        public void ResetJuggernaut()
        {
            Juggernaut.Info("Resetting Juggernaut.");
            Juggernaut.juggernaut = null;
            Juggernaut.juggernaut_prevRank = null;
            Juggernaut.juggernaut_health = 0;
        }

        public void EndGamemodeRound()
        {
            Juggernaut.Info("EndgameRound Function");
            ResetJuggernaut();
            Juggernaut.roundstarted = false;
            Juggernaut.Server.Round.EndRound();
        }

        public IEnumerable<float> SpawnAsNTFCommander(Player player)
        {
            player.ChangeRole(Role.NTF_COMMANDER, false, true, false, false);
            yield return 2;

            foreach (Smod2.API.Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.SetHealth(150);
            player.SetAmmo(AmmoType.DROPPED_5, 500);
            player.SetAmmo(AmmoType.DROPPED_7, 500);
            player.SetAmmo(AmmoType.DROPPED_9, 500);
            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.RADIO);
            player.GiveItem(ItemType.E11_STANDARD_RIFLE);
            player.GiveItem(ItemType.FLASHBANG);
            player.GiveItem(ItemType.MEDKIT);
            player.GiveItem(ItemType.MTF_COMMANDER_KEYCARD);
            player.GiveItem(ItemType.FRAG_GRENADE);
            if (Juggernaut.NTF_Disarmer) player.GiveItem(ItemType.DISARMER);

            int ran = Juggernaut.gen.Next(1, 100);
            if (ran > 75) player.GiveItem(ItemType.MICROHID);

            player.PersonalClearBroadcasts();
            if (Juggernaut.juggernaut != null)
                player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut " + Juggernaut.juggernaut.Name + "</color>", false);
            else
                player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut</color>", false);
        }

        public void SpawnAsJuggernaut(Player player)
        {
            Juggernaut.juggernaut = player;

            //Spawned as Juggernaut in 939s spawn location
            Vector spawn = Juggernaut.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);
            player.ChangeRole(Role.CHAOS_INSURGENCY, false, false, true, true);
            player.Teleport(spawn);

            Juggernaut.juggernaut_prevRank = new string[] { player.GetUserGroup().Color, player.GetUserGroup().Name };

            // Given a Juggernaut badge
            player.SetRank("silver", "Juggernaut");

            // Health scales with amount of players in round
            int health = Juggernaut.Jugg_base + (Juggernaut.Jugg_increase * Juggernaut.Server.NumPlayers) - 500;
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

        public string DrawHealthBar(double percentage)
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
}