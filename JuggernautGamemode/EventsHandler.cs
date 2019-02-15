using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using System;
using System.Timers;
using UnityEngine;

namespace JuggernautGamemode
{
    internal class EventsHandler : IEventHandlerReload, IEventHandlerWaitingForPlayers, IEventHandlerSetSCPConfig, IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerPlayerHurt, IEventHandlerSetRoleMaxHP, IEventHandlerSetRole,
        IEventHandlerLure, IEventHandlerContain106, IEventHandlerThrowGrenade
    {
        private readonly Juggernaut plugin;

        public EventsHandler(Juggernaut plugin) => this.plugin = plugin;

        public Player juggernaut;
        public Player activator = null;
        private int juggernaut_healh;
        private int ntf_health;
        //private bool blackouts = false;
        private string[] juggernaut_prevRank = new string[2];
        public static Player selectedJuggernaut = null;
        private float critical_damage;
        public static Timer timer;
        public static Player jugg_killer = null;
        public static int mtf_count = 0;

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (!Juggernaut.roundstarted)
                {
                    Server server = plugin.pluginManager.Server;
                    server.Map.ClearBroadcasts();
                    server.Map.Broadcast(25, "<color=#228B22>Juggernaut Gamemode</color> is starting...", false);
                }
            }
        }

        public void OnSetRoleMaxHP(SetRoleMaxHPEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (ev.Role == Role.CHAOS_INSURGENCY)
                    ev.MaxHP = juggernaut_healh;
            }
        }

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (IsJuggernaut(ev.Player))
                {
                    if (ev.TeamRole.Team != Smod2.API.Team.CHAOS_INSURGENCY || ev.TeamRole.Team == Smod2.API.Team.SPECTATOR)
                    {
                        ResetJuggernaut(ev.Player);
                    }
                }
                else
                {
                    // Set NTF Inventory
                    plugin.Info("Setting NTF items..");
                    List<ItemType> items = new List<ItemType>();
                    items.Add(ItemType.MICROHID);
                    items.Add(ItemType.E11_STANDARD_RIFLE);
                    items.Add(ItemType.MTF_COMMANDER_KEYCARD);
                    items.Add(ItemType.FRAG_GRENADE);
                    items.Add(ItemType.FLASHBANG);
                    items.Add(ItemType.RADIO);
                    items.Add(ItemType.MEDKIT);

                    if (Juggernaut.NTF_Disarmer)

                    {
                        items.Add(ItemType.DISARMER);
                    }
                    else
                    {

                        items.Add(ItemType.FRAG_GRENADE);
                    }

                    if (ev.TeamRole.Team != Smod2.API.Team.SPECTATOR)
                    {
                        if (ev.TeamRole.Team != Smod2.API.Team.NINETAILFOX)
                        {
                            plugin.Info("Spawning " + ev.Player.Name + " as NTF Commander, and setting inventory.");
                            ev.Items = items;
                            SpawnAsNTFCommander(ev.Player);
                        }
                        else if (ev.TeamRole.Role == Role.FACILITY_GUARD || ev.TeamRole.Role == Role.NTF_LIEUTENANT || ev.TeamRole.Role == Role.NTF_SCIENTIST || ev.TeamRole.Role == Role.NTF_CADET)
                            ev.Items = items;
                        SpawnAsNTFCommander(ev.Player);
                    }
                }
            }
        }

        public void OnReload(PlayerReloadEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (ev.Player.Name == juggernaut.Name || ev.Player.SteamId == juggernaut.SteamId)
                {
                    ev.Player.SetAmmo(AmmoType.DROPPED_7, 2000);
                }
            }
        }

        public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (ev.Player == juggernaut)
                {
                    if (Juggernaut.jugg_infinite_nades)
                    {
                        ev.Player.GiveItem(ItemType.FRAG_GRENADE);
                    }
                }
            }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Juggernaut.Jugg_base = this.plugin.GetConfigInt("juggernaut_base_health");
            Juggernaut.Jugg_increase = this.plugin.GetConfigInt("juggernaut_increase_amount");
            Juggernaut.NTF_Disarmer = this.plugin.GetConfigBool("juggernaut_ntf_disarmer");
            Juggernaut.Jugg_grenade = this.plugin.GetConfigInt("juggernaut_jugg_grenades");
            Juggernaut.NTF_Health = this.plugin.GetConfigInt("juggernaut_ntf_health");
            critical_damage = plugin.GetConfigFloat("juggernaut_critical_damage");
            Juggernaut.jugg_infinite_nades = this.plugin.GetConfigBool("juggernaut_infinite_jugg_nades");

            //foreach (Plugin p in PluginManager.Manager.EnabledPlugins)
            //{
            //    if (p.Details.id == "blackout")
            //    {
            //        if (p is Blackout.Plugin)
            //        {
            //            if (Blackout.Plugin.enabled)
            //            {
            //                Blackout.Plugin.DisableBlackouts();
            //                blackouts = true;
            //            }
            //        }
            //    }
            //}
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Juggernaut.enabled)
            {
                Juggernaut.roundstarted = true;
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Juggernaut Gamemode Started!");
                List<Player> players = ev.Server.GetPlayers();
                
                if (jugg_killer != null)
                {
                    selectedJuggernaut = jugg_killer;
                }

                if (selectedJuggernaut == null)
                {
                    int chosenJuggernaut = new System.Random().Next(players.Count);

                    juggernaut = players[chosenJuggernaut];

                    foreach (Player player in players)
                    {
                        // Selected random Juggernaut
                        if (IsJuggernaut(player))
                        {
                            plugin.Info("" + player.Name + " Chosen as the Juggernaut");
                            SpawnAsJuggernaut(player);
                        }
                        else
                        {
                            // Spawned as normal NTF Commander
                            plugin.Debug("Spawning " + player.Name + "as an NTF Commander");
                            SpawnAsNTFCommander(player);
                        }
                    }
                }
                else if (selectedJuggernaut != null && selectedJuggernaut is Player)
                {
                    foreach (Player player in players)
                    {
                        if (player.SteamId == selectedJuggernaut.SteamId || player.Name == selectedJuggernaut.Name)
                        {
                            plugin.Info("Selected " + selectedJuggernaut.Name + " as the Juggernaut");
                            SpawnAsJuggernaut(player);
                            selectedJuggernaut = null;
                        }
                        else
                        {
                            plugin.Debug("Spawning " + player.Name + "as an NTF Commander");
                            SpawnAsNTFCommander(player);
                        }
                    }
                }
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Juggernaut.enabled)
                plugin.Info("Round Ended!");
                EndGamemodeRound();
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Juggernaut.enabled)
            {
                bool juggernautAlive = false;
                bool mtfAllive = false;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (IsJuggernaut(player) && player.TeamRole.Team == Smod2.API.Team.CHAOS_INSURGENCY)
                    {
                        juggernautAlive = true; continue;
                    }

                    else if (player.TeamRole.Team == Smod2.API.Team.NINETAILFOX)
                        mtfAllive = true;
                        mtf_count = mtf_count++;
                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (juggernaut != null && juggernautAlive && mtfAllive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (juggernaut != null && juggernautAlive && mtfAllive == false)
                    {
                        ev.Status = ROUND_END_STATUS.CI_VICTORY; EndGamemodeRound();
                    }
                    else if (juggernaut == null && juggernautAlive == false && mtfAllive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY; EndGamemodeRound();
                    }
                }
            }
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (IsJuggernaut(ev.Player))
                {
                    plugin.pluginManager.Server.Map.ClearBroadcasts();
                    plugin.pluginManager.Server.Map.Broadcast(20, "<color=#228B22>Juggernaut " + juggernaut.Name + "</color> has been killed by" + ev.Killer.Name + "!", false);
                    ResetJuggernaut(ev.Player);
                    jugg_killer = ev.Killer;
                }
                else
                {
                    mtf_count = mtf_count--;
                    plugin.Server.Map.ClearBroadcasts();
                    plugin.Server.Map.Broadcast(15, "There are " + mtf_count + " NTF remaining.", false);
                }   
            }
        }

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (IsJuggernaut(ev.Player))
                {
                    juggernaut_healh = (juggernaut_healh > ev.Player.GetHealth()) ? juggernaut_healh : ev.Player.GetHealth();
                    plugin.pluginManager.Server.Map.ClearBroadcasts();
                    plugin.pluginManager.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + juggernaut.Name + "</color> HP : <color=#ff0000>" + (Convert.ToInt32(juggernaut.GetHealth() - ev.Damage)) + "/" + juggernaut_healh + "</color>", false);
                }
            }
        }

        public void OnLure(PlayerLureEvent ev)
        {
            if (Juggernaut.enabled)
            {
                activator = ev.Player;
            }

        }

        public void OnContain106(PlayerContain106Event ev)
        {
            if (Juggernaut.enabled)
            {
                if (juggernaut != null)
                {
                    timer = new Timer();
                    timer.Interval = 10000;
                    timer.Elapsed += OnTimedEvent;
                    timer.AutoReset = false;
                    timer.Enabled = true;
                }
            }
        }

        public void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            Player juggernautPlayer = GetJuggernautPlayer();
            if (juggernautPlayer != null && activator != null)
                CriticalHitJuggernaut(juggernautPlayer, activator);
            else if (juggernautPlayer != null)
                CriticalHitJuggernaut(juggernautPlayer);
        }

        public void OnSetSCPConfig(SetSCPConfigEvent ev)
        {
            if (Juggernaut.enabled)
            {
                ev.Ban049 = true;
                ev.Ban079 = true;
                ev.Ban096 = true;
                ev.Ban106 = true;
                ev.Ban173 = true;
                ev.Ban939_53 = true;
                ev.Ban939_89 = true;
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            if (Juggernaut.enabled)
                ev.SpawnChaos = false;
        }

        public bool IsJuggernaut(Player player)
        {
            if (juggernaut != null)
            {
                if (player.Name == juggernaut.Name || player.SteamId == juggernaut.SteamId)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public Player GetJuggernautPlayer()
        {
            foreach (Player player in plugin.pluginManager.Server.GetPlayers())
            {
                if (IsJuggernaut(player))
                {
                    return player;
                }
                else
                {
                    plugin.Warn("Juggernaut not found!");
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
            int damage = (int)(juggernaut_healh * critical_damage);
            player.Damage(damage, DamageType.FRAG);
            player.Teleport(position);
            plugin.pluginManager.Server.Map.Broadcast(10, "The <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
            plugin.Debug("Juggernaut Disarmed & Teleported");
        }

        public void CriticalHitJuggernaut(Player player, Player activator)
        {
            //Vector position = PluginManager.Manager.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
            Vector position = GetRandomPDExit();
            int damage = (int)(juggernaut_healh * critical_damage);
            player.Damage(damage, DamageType.FRAG);
            player.Teleport(position);
            plugin.pluginManager.Server.Map.Broadcast(10, "" + activator.Name + " has sacrifieced themselves and made the <color=#228B22>Juggernaut</color> take a <b>critical hit <i><color=#ff0000> -" + damage + "</color></i></b> and has been <b>transported</b> across the facility!", false);
            plugin.Debug("Juggernaut Disarmed & Teleported");
        }

        public void ResetJuggernaut(Player player)
        {
            if (juggernaut_prevRank != null && juggernaut_prevRank.Length == 2)
                player.SetRank(juggernaut_prevRank[0], juggernaut_prevRank[1]);
            else
                juggernaut.SetRank();
            ResetJuggernaut();
        }

        public void ResetJuggernaut()
        {
            plugin.Info("Resetting Juggernaut.");
            juggernaut = null;
            juggernaut_prevRank = null;
            juggernaut_healh = 0;
        }

        public void EndGamemodeRound()
        {
            if (Juggernaut.enabled)
            {
                plugin.Info("EndgameRound Function");
                ResetJuggernaut();
                Juggernaut.roundstarted = false;
                plugin.Server.Round.EndRound();
                //if (blackouts)
                //    Blackout.Plugin.EnableBlackouts();
            }

        }

        public void SpawnAsNTFCommander(Player player)
        {
            player.ChangeRole(Role.NTF_COMMANDER, false, true, true, true);

            ntf_health = Juggernaut.NTF_Health;
            plugin.Info("SpawnNTF Health");
            player.SetHealth(ntf_health);

           player.PersonalClearBroadcasts();
            if (juggernaut != null)
                player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut " + juggernaut.Name + "</color>", false);
            else
                player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut</color>", false);
        }

        public void SpawnAsJuggernaut(Player player)
        {
            juggernaut = player;

            //Spawned as Juggernaut in 939s spawn location
            Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);
            player.ChangeRole(Role.CHAOS_INSURGENCY, false, false, true, true);
            player.Teleport(spawn);

            juggernaut_prevRank = new string[] { player.GetUserGroup().Color, player.GetUserGroup().Name };

            // Given a Juggernaut badge
            player.SetRank("silver", "Juggernaut");

            // Health scales with amount of players in round
            int health = Juggernaut.Jugg_base + (Juggernaut.Jugg_increase * plugin.Server.NumPlayers ) - 500;
            player.SetHealth(health);
            juggernaut_healh = health;

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
    }
}