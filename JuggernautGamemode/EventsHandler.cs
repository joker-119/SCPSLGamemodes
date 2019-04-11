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
	internal class EventsHandler : IEventHandlerReload, IEventHandlerWaitingForPlayers, IEventHandlerSetSCPConfig, IEventHandlerTeamRespawn, IEventHandlerCheckRoundEnd, IEventHandlerRoundStart,
		IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerRoundEnd, IEventHandlerPlayerHurt, IEventHandlerSetRoleMaxHP, IEventHandlerSetRole, IEventHandlerLure, IEventHandlerContain106,
		IEventHandlerThrowGrenade, IEventHandlerRoundRestart
	{
		private readonly Juggernaut plugin;

		public EventsHandler(Juggernaut plugin) => this.plugin = plugin;

		public static Timer timer;

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (plugin.Enabled)
			{
				if (!plugin.RoundStarted)
				{
					Server server = plugin.Server;
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
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			if (ev.Role == Role.CHAOS_INSURGENCY)
				ev.MaxHP = plugin.JuggHealth;
		}

		public void OnReload(PlayerReloadEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.Name == plugin.Jugg.Name || ev.Player.SteamId == plugin.Jugg.SteamId)
			{
				ev.Player.SetAmmo(AmmoType.DROPPED_7, 2000);
				ev.Player.SetAmmo(AmmoType.DROPPED_5, 2000);
				ev.Player.SetAmmo(AmmoType.DROPPED_9, 2000);
			}
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (!plugin.RoundStarted) return;

			if (ev.Player.SteamId == plugin.Jugg.SteamId && plugin.JuggInfiniteNades)
			{
				ev.Player.GiveItem(ItemType.FRAG_GRENADE);
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.ReloadConfig();
		}
		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (!plugin.Enabled || !plugin.RoundStarted) return;
			if (!plugin.Functions.IsJuggernaut(ev.Player)) return;

			if (ev.TeamRole.Team != Smod2.API.Team.CHAOS_INSURGENCY || ev.TeamRole.Team == Smod2.API.Team.SPECTATOR)
				plugin.Functions.ResetJuggernaut();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (plugin.Enabled)
			{
				plugin.RoundStarted = true;
				plugin.Server.Map.ClearBroadcasts();
				plugin.Info("Juggernaut Gamemode Started!");

				List<Player> players = ev.Server.GetPlayers();

				if (plugin.SelectedJugg == null)
				{
					int limit = 50;
					for (int i = 0; i < limit; i++)
					{
						int chosenJuggernaut = plugin.Gen.Next(players.Count);
						if (!(players[chosenJuggernaut].OverwatchMode))
						{
							plugin.Jugg = players[chosenJuggernaut];
							break;
						}
					}

					foreach (Player player in players)
					{
						// Selected random Juggernaut
						if (plugin.Functions.IsJuggernaut(player))
						{
							plugin.Info("" + player.Name + " Chosen as the Juggernaut");
							plugin.Functions.SpawnAsJuggernaut(player);
						}
						else
						{
							// Spawned as normal NTF Commander
							plugin.Debug("Spawning " + player.Name + "as an NTF Commander");
							Timing.Run(plugin.Functions.SpawnAsNTFCommander(player));
						}
					}
				}
				else if (plugin.SelectedJugg != null && plugin.SelectedJugg is Player)
				{
					foreach (Player player in players)
					{
						if (player.SteamId == plugin.SelectedJugg.SteamId || player.Name == plugin.SelectedJugg.Name)
						{
							plugin.Info("Selected " + plugin.SelectedJugg.Name + " as the Juggernaut");
							plugin.Functions.SpawnAsJuggernaut(player);
							plugin.SelectedJugg = null;
						}
						else
						{
							plugin.Debug("Spawning " + player.Name + "as an NTF Commander");
							Timing.Run(plugin.Functions.SpawnAsNTFCommander(player));
						}
					}
				}

				for (int i = 0; i < 4 && players.Count > 0; i++)
				{
					foreach (Player player in players)
					{
						if (player.SteamId == plugin.Jugg.SteamId) continue;
						player.GiveItem(ItemType.MICROHID);
					}
				}
			}
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Ended!");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Round Restarted.");
			plugin.Functions.EndGamemodeRound();
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!plugin.RoundStarted) return;


			bool juggernautAlive = false;
			bool mtfAllive = false;

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (plugin.Functions.IsJuggernaut(player) && player.TeamRole.Team == Smod2.API.Team.CHAOS_INSURGENCY)
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
				if (plugin.Jugg != null && juggernautAlive && mtfAllive)
				{
					ev.Status = ROUND_END_STATUS.ON_GOING;
				}
				else if (plugin.Jugg != null && juggernautAlive && mtfAllive == false)
				{
					ev.Status = ROUND_END_STATUS.CI_VICTORY; plugin.Functions.EndGamemodeRound();
				}
				else if (plugin.Jugg == null && juggernautAlive == false && mtfAllive)
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY; plugin.Functions.EndGamemodeRound();
				}
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			if (plugin.Functions.IsJuggernaut(ev.Player))
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(20, "<color=#228B22>Juggernaut " + plugin.Jugg.Name + "</color> has been killed by " + ev.Killer.Name + "!", false);

				plugin.Functions.ResetJuggernaut(ev.Player);
			}
			else
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(15, "There are " + (plugin.Server.Round.Stats.NTFAlive - 1) + " NTF remaining.", false);
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			if (plugin.Functions.IsJuggernaut(ev.Player))
			{
				plugin.Server.Map.ClearBroadcasts();

				plugin.JuggHealth = (plugin.JuggHealth > ev.Player.GetHealth()) ? plugin.JuggHealth : ev.Player.GetHealth();
				int currentHealth = Convert.ToInt32(plugin.Jugg.GetHealth() - ev.Damage);
				int maxHealth = plugin.JuggHealth;
				double percentage = Math.Round((double)currentHealth / (double)maxHealth, 2);

				switch (plugin.HealthBarType)
				{
					default:
					case Juggernaut.HealthBar.Raw:
						plugin.Debug("Raw Health Bar Created");
						plugin.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + plugin.Jugg.Name + "</color> HP : <color=#ff0000>" + currentHealth + "/" + maxHealth + "</color>", false); break;
					case Juggernaut.HealthBar.Bar:
						plugin.Debug("Drawn Bar Health Bar Created");
						string bar = plugin.Functions.DrawHealthBar(percentage);
						plugin.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + plugin.Jugg.Name + "</color> HP : <color=#ff0000>" + bar + "</color>", false); break;
					case Juggernaut.HealthBar.Percentage:
						plugin.Debug("Percentage Health Bar Created");
						plugin.Server.Map.Broadcast(3, "<color=#228B22>Juggernaut " + plugin.Jugg.Name + "</color> HP : <color=#ff0000>" + (percentage * 100) + "%</color>", false); break;
				}
			}
		}

		public void OnLure(PlayerLureEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			plugin.Activator = ev.Player;

		}

		public void OnContain106(PlayerContain106Event ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			if (plugin.Jugg != null)
			{
				timer = new Timer();
				timer.Interval = 10000;
				timer.Elapsed += OnTimedEvent;
				timer.AutoReset = false;
				timer.Enabled = true;
			}
		}

		public void OnTimedEvent(System.Object source, ElapsedEventArgs e)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			Player juggernautPlayer = plugin.Functions.GetJuggernautPlayer();

			if (juggernautPlayer != null && plugin.Activator != null)
				plugin.Functions.CriticalHitJuggernaut(juggernautPlayer, plugin.Activator);
			else if (juggernautPlayer != null)
				plugin.Functions.CriticalHitJuggernaut(juggernautPlayer);
		}

		public void OnSetSCPConfig(SetSCPConfigEvent ev)
		{
			if (!plugin.Enabled && !plugin.RoundStarted) return;

			ev.Ban049 = true;
			ev.Ban079 = true;
			ev.Ban096 = true;
			ev.Ban106 = true;
			ev.Ban173 = true;
			ev.Ban939_53 = true;
			ev.Ban939_89 = true;
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (!plugin.RoundStarted) return;

			plugin.Info("Jugg Respawn.");
			List<Player> respawn = new List<Player>();

			foreach (Player player in ev.PlayerList)
			{
				respawn.Add(player);
			}

			ev.PlayerList = new List<Player>();
			ev.PlayerList = respawn;

			ev.SpawnChaos = false;

			foreach (Player player in ev.PlayerList)
			{
				Timing.Run(plugin.Functions.SpawnAsNTFCommander(player));
			}

			ev.PlayerList.Clear();
		}
	}
}