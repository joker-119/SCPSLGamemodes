using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using scp4aiur;

namespace Bomber
{
	internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDie, IEventHandlerPlayerHurt, IEventHandlerCheckRoundEnd
	{
		private readonly Bomber plugin;
		public EventsHandler(Bomber plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			Bomber.spawn_class = this.plugin.GetConfigString("bomb_class");
			Bomber.medkits = this.plugin.GetConfigBool("bomb_medkits");
			Bomber.min = this.plugin.GetConfigInt("bomb_min");
			Bomber.max = this.plugin.GetConfigInt("bomb_max");
		}
		public void OnRoundStart(RoundStartEvent ev)
		{
			if (!Bomber.enabled) return;
			Bomber.roundstarted = true;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Info("Bomberman Gamemode started!");
			Bomber.players = plugin.Server.GetPlayers();
			
			if (!(Bomber.spawn_class == ""))
			{
				switch (Bomber.spawn_class.ToLower().Trim())
				{
					case "classd":
						plugin.Debug("Class-D spawn selected.");
						foreach (Player players in Bomber.players)
							players.ChangeRole(Role.CLASSD, false, true, false, false);
						break;
					case "sci":
					case "nerd":
					case "scientist":
						plugin.Debug("Scientists spawn selected.");
						foreach (Player player in Bomber.players)
							player.ChangeRole(Role.SCIENTIST, false, true, false, false);
						break;
					case "guard":
						plugin.Debug("Guard spawn selected.");
						foreach (Player player in Bomber.players)
							player.ChangeRole(Role.FACILITY_GUARD, false, true, false, false);
						break;
					case "ntf":
						plugin.Debug("NTF spawn selected.");
						foreach (Player player in Bomber.players)
							player.ChangeRole(Role.NTF_COMMANDER, false, true, false, false);
						break;
					case "chaos":
						plugin.Debug("Chaos spawn selected.");
						foreach (Player player in Bomber.players)
							player.ChangeRole(Role.CHAOS_INSURGENCY, false, true, false, false);
						break;
					case "war":
						plugin.Debug("Warmode initiated.");
						Bomber.warmode = true;
						List<string> nerds = new List<string>();
						int ran = Bomber.gen.Next(Bomber.players.Count);
						for (int i = 0; i < Bomber.players.Count / 2; i++)
						{
							nerds.Add(Bomber.players[ran].Name);
						}
						foreach (Player player in Bomber.players)
						{
							if (nerds.Contains(player.Name))
								player.ChangeRole(Role.SCIENTIST, false, true, false, false);
							else
								player.ChangeRole(Role.CLASSD, false, true, false, false);
						}
						break;
					case "":
					default:
						plugin.Debug("Normal round selected.");
						break;
				}
			}
			Timing.Run(Functions.singleton.SpawnGrenades(30));
		}
		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!Bomber.enabled && !Bomber.roundstarted) return;
			Functions.singleton.EndGamemodeRound();			
		}
		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (ev.Player == ev.Attacker && ev.DamageType == DamageType.FRAG && Bomber.warmode)
				ev.Damage = 0;
		}
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(10, "There are " + (Bomber.players.Count - 1) + " players alive!", false);
			Bomber.players.Remove(ev.Player);
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
					alive_count++;
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
			else if (alive_count < 1)
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
