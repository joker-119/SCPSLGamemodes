using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using scp4aiur;

namespace Bomber
{
	internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie, IEventHandlerSetRole, IEventHandlerPlayerHurt, IEventHandlerCheckRoundEnd, IEventHandlerPlayerJoin
	{
		private readonly Bomber plugin;
		public EventsHandler(Bomber plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			Bomber.spawn_class = this.plugin.GetConfigString("bomb_class");
			Bomber.medkits = this.plugin.GetConfigBool("bomb_medkits");
			Bomber.min = this.plugin.GetConfigInt("bomb_min");
			Bomber.max = this.plugin.GetConfigInt("bomb_max");
			Bomber.grenade_multi = this.plugin.GetConfigFloat("bomb_grenade_multi");
		}
		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!Bomber.enabled) return;
			Bomber.roundstarted = true;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Info("Bomberman Gamemode started!");
			List<Player> players = plugin.Server.GetPlayers();
			
			if (!(Bomber.spawn_class == ""))
			{
				switch (Bomber.spawn_class.ToLower().Trim())
				{
					case "classd":
						plugin.Info("Class-D spawn selected.");
						foreach (Player player in players)
						{
							player.ChangeRole(Role.CLASSD, false, true, false, false);
						}
						break;
					case "sci":
					case "nerd":
					case "scientist":
						plugin.Info("Scientists spawn selected.");
						foreach (Player player in players)
						{
							player.ChangeRole(Role.SCIENTIST, false, true, false, false);
						}
						break;
					case "guard":
						plugin.Info("Guard spawn selected.");
						foreach (Player player in players)
						{
							player.ChangeRole(Role.FACILITY_GUARD, false, true, false, false);
						}
						break;
					case "ntf":
						plugin.Info("NTF spawn selected.");
						foreach (Player player in players)
						{
							player.ChangeRole(Role.NTF_COMMANDER, false, true, false, false);
						}
						break;
					case "chaos":
						plugin.Info("Chaos spawn selected.");
						foreach (Player player in players)
						{
							player.ChangeRole(Role.CHAOS_INSURGENCY, false, true, false, false);
						}
						break;
					case "war":
						plugin.Info("Warmode initiated.");
						Bomber.warmode = true;
						List<string> nerds = new List<string>();
						int num = (players.Count / 2);
						if (num == 0) 
							plugin.Error("Ya dun goofed, kid!");
						for (int i = 0; i < num; i++)
						{
							int ran = Bomber.gen.Next(players.Count);
							nerds.Add(players[ran].SteamId);
							players.Remove(players[ran]);
						}
						foreach (Player player in players)
						{
							if (nerds.Contains(player.SteamId))
								player.ChangeRole(Role.SCIENTIST, false, true, false, false);
							else
								player.ChangeRole(Role.CLASSD, false, true, false, false);
						}
						break;
					case "":
					default:
						plugin.Info("Normal round selected.");
						break;
				}
			}
			Timing.Run(Functions.singleton.SpawnGrenades(30));
		}
		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (!Bomber.enabled || !Bomber.roundstarted) return;
			if (Bomber.warmode)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(15, "Half of you are Class-D, the other half are nerds. Yor goal is to kill the opposing team with your grenades!", false);
			}
			else
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(15, "Grenades will start spawning under you shortly. Survive the rain of fire!", false);	
			}
		}
		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!Bomber.enabled && !Bomber.roundstarted) return;
			plugin.Info("Round Ended.");
			Functions.singleton.EndGamemodeRound();			
		}
		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!Bomber.enabled || Bomber.roundstarted) return;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(25, "<color=#c50000>Bomberman Gamemode</color> is starting..", false);
		}
		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (!Bomber.enabled || !Bomber.roundstarted) return;
			if (ev.Player.SteamId == ev.Attacker.SteamId && Bomber.warmode)
					ev.Damage = 0;
			ev.Damage = (ev.Damage * Bomber.grenade_multi);
		}
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (!Bomber.enabled && !Bomber.roundstarted) return;
			if (ev.Player.Name == "Sever" || ev.Player.Name == "") return;
			List<Player> players = plugin.Server.GetPlayers();
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(10, "There are " + (players.Count - 1) + " players alive!", false);
		}
		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (!Bomber.enabled && !Bomber.roundstarted) return;
			bool classdAlive = false;
			bool sciAlive = false;
			int alive_count = 0;

			foreach (Player player in ev.Server.GetPlayers())
			{
				if (player.TeamRole.Team == Team.CLASSD)
				{
					classdAlive = true;
					continue;
				}
				else if (player.TeamRole.Team == Team.SCIENTIST)
				{
					sciAlive = true;
				}
				if (Functions.singleton.IsAlive(player))
				{
					alive_count++;
				}
			}
			if (ev.Server.GetPlayers().Count < 1) return;
			if (Bomber.warmode)
			{
				if (classdAlive && sciAlive)
					ev.Status = ROUND_END_STATUS.ON_GOING;
				else if (classdAlive && !sciAlive)
				{
					ev.Status = ROUND_END_STATUS.CI_VICTORY; Functions.singleton.EndGamemodeRound();
				}
				else if (!classdAlive && sciAlive)
				{
					ev.Status = ROUND_END_STATUS.MTF_VICTORY; Functions.singleton.EndGamemodeRound();
				}
			}
			else if (alive_count > 1)
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (alive_count == 1)
			{
				ev.Status = ROUND_END_STATUS.NO_VICTORY; Functions.singleton.EndGamemodeRound();
				foreach (Player player in ev.Server.GetPlayers())
				{
					if (player.TeamRole.Team != Team.SPECTATOR)
					{
						ev.Server.Map.ClearBroadcasts();
						ev.Server.Map.Broadcast(10, player.Name + " Winner, winner, chicken.. Oh you still exploded.", false);
					}
				}
			}
		}
	}
}
