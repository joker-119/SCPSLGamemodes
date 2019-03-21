using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System;


namespace Gangwar
{
    public class Functions
    {
        public static Functions singleton;
        public Gangwar Gangwar;
        public Functions(Gangwar plugin)
        {
            this.Gangwar = plugin;
            Functions.singleton = this;
        }
        public void EnableGamemode()
        {
            Gangwar.enabled = true;
            if (!Gangwar.roundstarted)
            {
                Gangwar.Server.Map.ClearBroadcasts();
                Gangwar.Server.Map.Broadcast(25, "<color=#00ffff> Gangwar Gamemode is starting..</color>", false);
            }
        }

        public void DisableGamemode()
        {
            Gangwar.enabled = false;
            Gangwar.Server.Map.ClearBroadcasts();
        }
        public void EndGamemodeRound()
        {
            Gangwar.Info("EndgameRound Function.");
            Gangwar.roundstarted = false;
            Gangwar.Server.Round.EndRound();
        }
        public IEnumerable<float> SpawnChaos(Player player, float delay)
        {
            yield return delay;
            player.ChangeRole(Role.CHAOS_INSURGENCY, false, true, false, true);
            yield return 2;
            foreach (Item item in player.GetInventory())
            {
                item.Remove();
            }
            player.GiveItem(ItemType.E11_STANDARD_RIFLE);
            player.GiveItem(ItemType.COM15);
            player.GiveItem(ItemType.FRAG_GRENADE);
            player.GiveItem(ItemType.MEDKIT);
            player.GiveItem(ItemType.FLASHBANG);

            player.SetAmmo(AmmoType.DROPPED_5, 500);
            player.SetAmmo(AmmoType.DROPPED_7, 500);
            player.SetAmmo(AmmoType.DROPPED_9, 500);
            player.SetHealth(Gangwar.ci_health);
        }
        public IEnumerable<float> SpawnNTF(Player player, float delay)
        {
            yield return delay;
            player.ChangeRole(Role.NTF_COMMANDER, false, true, false, false);
            yield return 2;
            foreach (Item item in player.GetInventory())
            {
                item.Remove();
            }
            player.GiveItem(ItemType.E11_STANDARD_RIFLE);
            player.GiveItem(ItemType.COM15);
            player.GiveItem(ItemType.FRAG_GRENADE);
            player.GiveItem(ItemType.MEDKIT);
            player.GiveItem(ItemType.FLASHBANG);

            player.SetAmmo(AmmoType.DROPPED_5, 500);
            player.SetAmmo(AmmoType.DROPPED_7, 500);
            player.SetAmmo(AmmoType.DROPPED_9, 500);
            player.SetHealth(Gangwar.ntf_health);
        }
    }
}