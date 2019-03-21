using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using System.Collections.Generic;
using scp4aiur;
using UnityEngine;

namespace Gungame
{
    internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerCheckRoundEnd, IEventHandlerThrowGrenade, IEventHandlerPlayerHurt, IEventHandlerWaitingForPlayers
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
            if (!GunGame.enabled) return;
            GunGame.roundstarted = true;
            List<Player> players = ev.Server.GetPlayers();
            foreach (Player player in players)
            {
                Timing.Run(Functions.singleton.Spawn(player));
                (player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
            }
        }
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (!GunGame.roundstarted) return;
            (ev.Player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
            Timing.Run(Functions.singleton.Spawn(ev.Player));
        }
        public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
        {
            if (!GunGame.enabled && !GunGame.roundstarted) return;
            ev.Player.GiveItem(ItemType.FRAG_GRENADE);
        }
        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (!GunGame.enabled && !GunGame.roundstarted) return;
            Functions.singleton.ReplaceGun(ev.Killer);
            if (!GunGame.reversed && ev.DamageTypeVar == DamageType.E11_STANDARD_RIFLE)
            {
                Functions.singleton.AnnounceWinner(ev.Killer);
                GunGame.winner = ev.Killer;
            }
            else if (GunGame.reversed && ev.DamageTypeVar == DamageType.FRAG)
            {
                Functions.singleton.AnnounceWinner(ev.Killer);
                GunGame.winner = ev.Killer;
            }
            else
                Timing.Run(Functions.singleton.Spawn(ev.Player));
        }
        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (!GunGame.enabled && !GunGame.roundstarted) return;
            if (ev.Player.SteamId == ev.Attacker.SteamId && ev.DamageType == DamageType.FRAG)
                ev.Damage = 0;

        }
        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (!GunGame.enabled && !GunGame.roundstarted) return;
            if (!(GunGame.winner is Player))
                ev.Status = ROUND_END_STATUS.ON_GOING;
        }
        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (!GunGame.enabled && !GunGame.roundstarted) return;
            Functions.singleton.EndGamemodeRound();
        }
    }
}