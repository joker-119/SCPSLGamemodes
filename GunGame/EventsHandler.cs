using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using System.Collections.Generic;
using scp4aiur;

namespace Gungame
{
	internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd,IEventHandlerSetConfig, IEventHandlerPlayerDie, IEventHandlerHandcuffed, IEventHandlerCheckRoundEnd, IEventHandlerThrowGrenade, IEventHandlerShoot, IEventHandlerWaitingForPlayers
	{
		private readonly GunGame plugin;

        public EventsHandler(GunGame plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			GunGame.zone = this.plugin.GetConfigString("gun_spawn_zone");
			GunGame.reversed = this.plugin.GetConfigBool("gun_reversed");
			GunGame.health = this.plugin.GetConfigInt("gun_health");
		}
		public void OnRoundStart(RoundStartEvent ev)
		{
			List<Player> players = ev.Server.GetPlayers();
			foreach (Player player in players)
			{
				Timing.Run(Functions.singleton.Spawn(player));
			}
		}
		public void OnShoot(PlayerShootEvent ev)
		{
			ev.Player.SetAmmo(AmmoType.DROPPED_5, 100);
			ev.Player.SetAmmo(AmmoType.DROPPED_7, 100);
			ev.Player.SetAmmo(AmmoType.DROPPED_9, 100);
		}
		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			ev.Player.GiveItem(ItemType.FRAG_GRENADE);
		}
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (GunGame.reversed)
			{
				foreach (Item item in ev.Killer.GetInventory())
				{
					switch (item.ItemType)
					{
						case ItemType.E11_STANDARD_RIFLE:
							item.Remove();
							ev.Killer.GiveItem(ItemType.P90);
							break;
						case ItemType.P90:
							item.Remove();
							ev.Killer.GiveItem(ItemType.LOGICER);
							break;
						case ItemType.LOGICER:
							item.Remove();
							ev.Killer.GiveItem(ItemType.MP4);
							break;
						case ItemType.MP4:
							item.Remove();
							ev.Killer.GiveItem(ItemType.USP);
							break;
						case ItemType.USP:
							item.Remove();
							ev.Killer.GiveItem(ItemType.COM15);
							break;
						case ItemType.COM15:
							item.Remove();
							ev.Killer.GiveItem(ItemType.FRAG_GRENADE);
							break;
						case ItemType.FRAG_GRENADE:
							item.Remove();
							ev.Killer.GiveItem(ItemType.DISARMER);
							break;
					}
				}
			}
			foreach (Item item in ev.Killer.GetInventory())
			{
				switch (item.ItemType)
				{
					case ItemType.FRAG_GRENADE:
						item.Remove();
						ev.Killer.GiveItem(ItemType.COM15);
						break;
					case ItemType.COM15:
						item.Remove();
						ev.Killer.GiveItem(ItemType.USP);
						break;
					case ItemType.USP:
						item.Remove();
						ev.Killer.GiveItem(ItemType.MP4);
						break;
					case ItemType.MP4:
						item.Remove();
						ev.Killer.GiveItem(ItemType.LOGICER);
						break;
					case ItemType.LOGICER:
						item.Remove();
						ev.Killer.GiveItem(ItemType.P90);
						break;
					case ItemType.P90:
						item.Remove();
						ev.Killer.GiveItem(ItemType.E11_STANDARD_RIFLE);
						break;
				}
			}
			if (!GunGame.reversed && ev.DamageTypeVar == DamageType.E11_STANDARD_RIFLE)
				Functions.singleton.AnnounceWinner(ev.Killer);
			else
				Timing.Run(Functions.singleton.Spawn(ev.Player));
		}
		public void OnHandcuffed(PlayerHandcuffedEvent ev)
		{
			if (GunGame.reversed)
			{
				Functions.singleton.AnnounceWinner(ev.Owner);
			}
			else 
				foreach (Item item in ev.Owner.GetInventory())
				{
					if (item.ItemType == ItemType.DISARMER)
					{
						item.Remove();
						ev.Owner.GiveItem(ItemType.FRAG_GRENADE);
					}
				}
		}
		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (GunGame.enabled || GunGame.roundstarted)
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}
		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (!GunGame.enabled || !GunGame.roundstarted) return;
			Functions.singleton.EndGamemodeRound();
		}
		public void OnSetConfig(SetConfigEvent ev)
		{
			if (!GunGame.enabled || !GunGame.roundstarted) return;
			if (ev.Key == "friendly_fire")
				ev.Value = true;
		}
	}
}