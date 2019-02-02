using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using System;
using System.Linq;
using System.Text;

namespace JuggernautGamemode
{
    internal class EventsHandler : IEventHandlerSetSCPConfig, IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerHandcuffed, IEventHandlerPlayerHurt, IEventHandlerDecideTeamRespawnQueue,
        IEventHandlerSetRoleMaxHP, IEventHandlerSetRole
    {
        private readonly Juggernaut plugin;

        public EventsHandler(Juggernaut plugin) => this.plugin = plugin;

        public Player juggernaut;
        private int juggernaut_healh;
        private string[] juggernaut_prevRank = new string[2];
        public static Player selectedJuggernaut = null;

        public void OnDecideTeamRespawnQueue(DecideRespawnQueueEvent ev)
        {
            if (Juggernaut.enabled)
                ev.Teams = new Team[] { Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX, Team.NINETAILFOX };
        }

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
            //if (Juggernaut.enabled)
            //{
            //    if (IsJuggernaut(ev.Player))
            //    {
            //        if (ev.TeamRole.Team != Team.CHAOS_INSURGENCY || ev.TeamRole.Team == Team.SPECTATOR)
            //        {
            //            ResetJuggernaut(ev.Player);
            //        }
            //    }
            //    else
            //    {
            //        if (ev.TeamRole.Team != Team.NINETAILFOX && ev.TeamRole.Team != Team.SPECTATOR)
            //        {
            //            SpawnAsNTFCommander(ev.Player);
            //        }
            //        //else if (ev.TeamRole.Role == Role.FACILITY_GUARD)
            //        //    SpawnAsNTFCommander(ev.Player);
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
                if (selectedJuggernaut == null)
                {
                    int chosenJuggernaut = new Random().Next(players.Count);

                    foreach (Player player in players)
                    {
                        // Selected random Juggernaut
                        if (players.IndexOf(player) == chosenJuggernaut)
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
                    if (IsJuggernaut(player) && player.TeamRole.Team == Team.CHAOS_INSURGENCY)
                    {
                        juggernautAlive = true; continue;
                    }

                    else if (player.TeamRole.Team == Team.NINETAILFOX)
                        mtfAllive = true;
                }

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

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Juggernaut.enabled)
            {
                if (IsJuggernaut(ev.Player))
                {
                    plugin.pluginManager.Server.Map.ClearBroadcasts();
                    plugin.pluginManager.Server.Map.Broadcast(20, "<color=#228B22>Juggernaut " + juggernaut.Name + "</color> has died!", false);
                    ResetJuggernaut(ev.Player);
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
                    plugin.pluginManager.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + juggernaut.Name + "</color> HP : <color=#ff0000>" + (juggernaut.GetHealth() - ev.Damage) + "/" + juggernaut_healh + "</color>", false);

                    if (ev.Attacker != ev.Player && ev.DamageType == DamageType.FRAG)
                        CriticalHitJuggernaut(ev.Player);
                }
            }
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

        public void OnHandcuffed(PlayerHandcuffedEvent ev)
        {
            //if (Juggernaut.enabled)
            //{
            //    plugin.Debug("OnHandCuffed Called");
            //    //if (juggernaut != null && ev.Player == juggernaut)
            //    if (IsJuggernaut(ev.Player))
            //    {
            //        plugin.Debug("Player is Juggernaut");
            //        ev.Handcuffed = false;

            //        List<PocketDimensionExit> exits = plugin.pluginManager.Server.Map.GetPocketDimensionExits();

            //        int chosenExit = new Random().Next(exits.Count);

            //        ev.Player.Teleport(exits[chosenExit].Position);
            //        ev.Player.SetHealth(ev.Player.GetHealth() - 1000);

            //        plugin.pluginManager.Server.Map.Broadcast(6, "The <color=#228B22>Juggernaut</color> took a <b>critical hit <i><color=#ff0000> - 100 HP</color></i></b> and has been <b>transported</b> across the facility!", false);
            //        plugin.Debug("Juggernaut Disarmed & Teleported");
            //    }
            //}
        }

        public bool IsJuggernaut(Player player)
        {
            if (juggernaut != null)
            {
                if (player.Name == juggernaut.SteamId || player.SteamId == juggernaut.SteamId)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public void CriticalHitJuggernaut(Player player)
        {
            List<PocketDimensionExit> exits = plugin.pluginManager.Server.Map.GetPocketDimensionExits();

            int chosenExit = new Random().Next(exits.Count);

            player.Teleport(exits[chosenExit].Position);
            player.Damage(1000, DamageType.FRAG);

            plugin.pluginManager.Server.Map.Broadcast(6, "The <color=#228B22>Juggernaut</color> took a <b>critical hit <i><color=#ff0000> - 100 HP</color></i></b> and has been <b>transported</b> across the facility!", false);
            plugin.Debug("Juggernaut Disarmed & Teleported");
        }

        public void ResetJuggernaut(Player player)
        {
            if (juggernaut_prevRank != null && juggernaut_prevRank.Length == 2)
                player.SetRank(juggernaut_prevRank[0], juggernaut_prevRank[1]);
            else
                juggernaut.SetRank();
            juggernaut = null;
            juggernaut_prevRank = null;
            juggernaut_healh = 0;
        }

        public void ResetJuggernaut()
        {
            juggernaut = null;
            juggernaut_prevRank = null;
            juggernaut_healh = 0;
        }

        public void EndGamemodeRound()
        {
            ResetJuggernaut();
            Juggernaut.roundstarted = false;
            //plugin.Server.Round.EndRound();

        }

        public void SpawnAsNTFCommander(Player player)
        {
            //// Clear Inventory
            //foreach (Item item in player.GetInventory())
            //    item.Remove();

            player.ChangeRole(Role.NTF_COMMANDER, true, true, true, true);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are an <color=#002DB3>NTF Commander</color> Work with others to eliminate the <color=#228B22>Juggernaut " + juggernaut.Name + "</color>", false);
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
            int health = 500 * plugin.Server.NumPlayers;
            player.SetHealth(health);
            juggernaut_healh = health;

            // Clear Inventory
            foreach (Item item in player.GetInventory())
                item.Remove();


            // 1 Logicer
            player.GiveItem(ItemType.LOGICER);

            // 1 O5 Keycard
            player.GiveItem(ItemType.O5_LEVEL_KEYCARD);

            // 6 Frag Grenades
            player.GiveItem(ItemType.FRAG_GRENADE);
            player.GiveItem(ItemType.FRAG_GRENADE);
            player.GiveItem(ItemType.FRAG_GRENADE);
            player.GiveItem(ItemType.FRAG_GRENADE);
            player.GiveItem(ItemType.FRAG_GRENADE);
            player.GiveItem(ItemType.FRAG_GRENADE);

            // 4,000 Reserve 7.72 Ammo
            player.SetAmmo(AmmoType.DROPPED_7, 4000);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are the <color=#228B22>Juggernaut</color> Eliminate all <color=#002DB3>NTF Commanders</color>", false);
        }
    }
}
