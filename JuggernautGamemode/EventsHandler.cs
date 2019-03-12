using Smod2.API;
using Smod2.EventHandlers;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using Smod2.Events;
using System;
using System.Timers;
using UnityEngine;
using scp4aiur;

namespace JuggernautGamemode
{
    internal class EventsHandler : IEventHandlerReload, IEventHandlerWaitingForPlayers, IEventHandlerSetSCPConfig, IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart, IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerPlayerHurt, IEventHandlerSetRoleMaxHP, IEventHandlerSetRole,
        IEventHandlerLure, IEventHandlerContain106, IEventHandlerThrowGrenade
    {
        private readonly Juggernaut plugin;

        public EventsHandler(Juggernaut plugin) => this.plugin = plugin;

        public static Timer timer;

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
                else
                {
                    ev.Player.PersonalClearBroadcasts();
                    ev.Player.PersonalBroadcast(10, "<b>Now Playing :</b> <color=#228B22>Juggernaut Gamemode</color>", false);
                }
            }
        }

        public void OnSetRoleMaxHP(SetRoleMaxHPEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                if (ev.Role == Role.CHAOS_INSURGENCY)
                    ev.MaxHP = Juggernaut.juggernaut_health;
            }
        }

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                if (Functions.singleton.IsJuggernaut(ev.Player))
                {
                    if (ev.TeamRole.Team != Smod2.API.Team.CHAOS_INSURGENCY || ev.TeamRole.Team == Smod2.API.Team.SPECTATOR)
                    {
                        Functions.singleton.ResetJuggernaut(ev.Player);
                    }
                }
                else
                {
                    // Set NTF Inventory
                    plugin.Info("Setting NTF items..");
                    List<ItemType> items = new List<ItemType>();
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
                            Timing.Run(Functions.singleton.SpawnAsNTFCommander(ev.Player));
                            ev.Player.SetHealth(Juggernaut.ntf_health);
                        }
                        else if (ev.TeamRole.Role == Role.FACILITY_GUARD || ev.TeamRole.Role == Role.NTF_LIEUTENANT || ev.TeamRole.Role == Role.NTF_SCIENTIST || ev.TeamRole.Role == Role.NTF_CADET)
                            ev.Items = items;
                        Timing.Run(Functions.singleton.SpawnAsNTFCommander(ev.Player));
                        ev.Player.SetHealth(Juggernaut.ntf_health);
                    }
                }
            }
        }

        public void OnReload(PlayerReloadEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted && Juggernaut.juggernaut != null)
            {
                if (ev.Player.Name == Juggernaut.juggernaut.Name || ev.Player.SteamId == Juggernaut.juggernaut.SteamId)
                {
                    ev.Player.SetAmmo(AmmoType.DROPPED_7, 2000);
					ev.Player.SetAmmo(AmmoType.DROPPED_5, 2000);
					ev.Player.SetAmmo(AmmoType.DROPPED_9, 2000);
                }
            }
        }

        public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                if (ev.Player.SteamId == Juggernaut.juggernaut.SteamId)
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
            Juggernaut.critical_damage = plugin.GetConfigFloat("juggernaut_critical_damage");
            Juggernaut.jugg_infinite_nades = this.plugin.GetConfigBool("juggernaut_infinite_jugg_nades");
            string type = this.plugin.GetConfigString("juggernaut_health_bar_type");
            switch (type.ToLower().Trim())
            {
                case "bar":
                    plugin.Debug("Drawn Bar Health Bar Selected");
                    Juggernaut.health_bar_type = HealthBar.Bar; break;
                case "percent":
                case "percentage":
                    plugin.Debug("Percentage Health Bar Selected");
                    Juggernaut.health_bar_type = HealthBar.Percentage; break;
                case "raw":
                default:
                    plugin.Debug("Raw Health Bar Selected");
                    Juggernaut.health_bar_type = HealthBar.Raw; break;
            }
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (Juggernaut.enabled)
            {
                Juggernaut.roundstarted = true;
                plugin.pluginManager.Server.Map.ClearBroadcasts();
                plugin.Info("Juggernaut Gamemode Started!");
                List<Player> players = ev.Server.GetPlayers();
                
                if (Juggernaut.jugg_killer != null && Juggernaut.jugg_killer is Player)
                {
                    Juggernaut.selectedJuggernaut = Juggernaut.jugg_killer;
                }

                if (Juggernaut.selectedJuggernaut == null)
                {
                    int chosenJuggernaut = new System.Random().Next(players.Count);

                    Juggernaut.juggernaut = players[chosenJuggernaut];

                    foreach (Player player in players)
                    {
                        // Selected random Juggernaut
                        if (Functions.singleton.IsJuggernaut(player))
                        {
                            plugin.Info("" + player.Name + " Chosen as the Juggernaut");
                            Functions.singleton.SpawnAsJuggernaut(player);
                        }
                        else
                        {
                            // Spawned as normal NTF Commander
                            plugin.Debug("Spawning " + player.Name + "as an NTF Commander");
							Timing.Run(Functions.singleton.SpawnAsNTFCommander(player));
                            
                        }
                    }
                }
                else if (Juggernaut.selectedJuggernaut != null && Juggernaut.selectedJuggernaut is Player)
                {
                    foreach (Player player in players)
                    {
                        if (player.SteamId == Juggernaut.selectedJuggernaut.SteamId || player.Name == Juggernaut.selectedJuggernaut.Name)
                        {
                            plugin.Info("Selected " + Juggernaut.selectedJuggernaut.Name + " as the Juggernaut");
                            Functions.singleton.SpawnAsJuggernaut(player);
                            Juggernaut.selectedJuggernaut = null;
							players.Remove(player);
                        }
                        else
                        {
                            plugin.Debug("Spawning " + player.Name + "as an NTF Commander");
                            Timing.Run(Functions.singleton.SpawnAsNTFCommander(player));
                        }
                    }
                }
				for (int i = 0; i < 4 && players.Count > 0; i++)
				{
					foreach (Player player in players)
					{
						player.GiveItem(ItemType.MICROHID);
						players.Remove(player);
					}
				}
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
                plugin.Info("Round Ended!");
                Functions.singleton.EndGamemodeRound();
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                bool juggernautAlive = false;
                bool mtfAllive = false;

                foreach (Player player in ev.Server.GetPlayers())
                {
                    if (Functions.singleton.IsJuggernaut(player) && player.TeamRole.Team == Smod2.API.Team.CHAOS_INSURGENCY)
                    {
                        juggernautAlive = true; continue;
                    }

                    else if (player.TeamRole.Team == Smod2.API.Team.NINETAILFOX)
                    {
                        mtfAllive = true;
                    }
                }
                if (ev.Server.GetPlayers().Count > 1)
                {
                    if (Juggernaut.juggernaut != null && juggernautAlive && mtfAllive)
                    {
                        ev.Status = ROUND_END_STATUS.ON_GOING;
                    }
                    else if (Juggernaut.juggernaut != null && juggernautAlive && mtfAllive == false)
                    {
                        ev.Status = ROUND_END_STATUS.CI_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                    else if (Juggernaut.juggernaut == null && juggernautAlive == false && mtfAllive)
                    {
                        ev.Status = ROUND_END_STATUS.MTF_VICTORY; Functions.singleton.EndGamemodeRound();
                    }
                }
            }
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                if (Functions.singleton.IsJuggernaut(ev.Player))
                {
                    plugin.pluginManager.Server.Map.ClearBroadcasts();
                    plugin.pluginManager.Server.Map.Broadcast(20, "<color=#228B22>Juggernaut " + Juggernaut.juggernaut.Name + "</color> has been killed by " + ev.Killer.Name + "!", false);
                    Functions.singleton.ResetJuggernaut(ev.Player);
                    Juggernaut.jugg_killer = ev.Killer;
                }
                else
                {
                    plugin.Server.Map.ClearBroadcasts();
                    plugin.Server.Map.Broadcast(15, "There are " + (Juggernaut.singleton.Server.Round.Stats.NTFAlive - 1) + " NTF remaining.", false);
                }   
            }
        }

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                if (Functions.singleton.IsJuggernaut(ev.Player))
                {
                    Juggernaut.juggernaut_health = (Juggernaut.juggernaut_health > ev.Player.GetHealth()) ? Juggernaut.juggernaut_health : ev.Player.GetHealth();
                    plugin.pluginManager.Server.Map.ClearBroadcasts();
                    int currentHealth = Convert.ToInt32(Juggernaut.juggernaut.GetHealth() - ev.Damage);
                    int maxHealth = Juggernaut.juggernaut_health;
                    double percentage = Math.Round((double)currentHealth / (double)maxHealth, 2);
                    switch (Juggernaut.health_bar_type)
                    {
                        default:
                        case HealthBar.Raw:
                            plugin.Debug("Raw Health Bar Created");
                            plugin.pluginManager.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + Juggernaut.juggernaut.Name + "</color> HP : <color=#ff0000>" + currentHealth + "/" + maxHealth + "</color>", false); break;
                        case HealthBar.Bar:
                            plugin.Debug("Drawn Bar Health Bar Created");
                            string bar = Functions.singleton.DrawHealthBar(percentage);
                            plugin.pluginManager.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + Juggernaut.juggernaut.Name + "</color> HP : <color=#ff0000>" + bar + "</color>", false); break;
                        case HealthBar.Percentage:
                            plugin.Debug("Percentage Health Bar Created");
                            plugin.pluginManager.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + Juggernaut.juggernaut.Name + "</color> HP : <color=#ff0000>" + (percentage * 100) + "%</color>", false); break;
                    }
                }
            }
        }

        public void OnLure(PlayerLureEvent ev)
        {
            if (Juggernaut.enabled && Juggernaut.roundstarted)
            {
                Juggernaut.activator = ev.Player;
            }

        }

        public void OnContain106(PlayerContain106Event ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                if (Juggernaut.juggernaut != null)
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
            if (Juggernaut.enabled || Juggernaut.roundstarted)
            {
                Player juggernautPlayer = Functions.singleton.GetJuggernautPlayer();
                if (juggernautPlayer != null && Juggernaut.activator != null)
                    Functions.singleton.CriticalHitJuggernaut(juggernautPlayer, Juggernaut.activator);
                else if (juggernautPlayer != null)
                    Functions.singleton.CriticalHitJuggernaut(juggernautPlayer);
            }
        }

        public void OnSetSCPConfig(SetSCPConfigEvent ev)
        {
            if (Juggernaut.enabled || Juggernaut.roundstarted)
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
            if (Juggernaut.enabled || Juggernaut.roundstarted)
                ev.SpawnChaos = false;
        }
    }
}