using Smod2;
using UnityEngine;
using Smod2.API;
using System.Collections.Generic;
using System;

namespace PresidentialEscortGamemode
{
	public class Functions
    {
		public static Functions singleton;
		public PresidentialEscort PresidentialEscort;
		public Functions(PresidentialEscort plugin)
		{
			this.PresidentialEscort = plugin;
			Functions.singleton = this;
		}
        public void EnableGamemode()
        {
            PresidentialEscort.enabled = true;
            if (!PresidentialEscort.roundstarted)
            {
                PresidentialEscort.pluginManager.Server.Map.ClearBroadcasts();
                PresidentialEscort.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting..", false);
            }
        }
        public void DisableGamemode()
        {
            PresidentialEscort.enabled = false;
            PresidentialEscort.pluginManager.Server.Map.ClearBroadcasts();
        }
        public void EndGamemodeRound()
        {
            PresidentialEscort.Info("EndgameRound Function");
            PresidentialEscort.roundstarted = false;
            PresidentialEscort.Server.Round.EndRound();
            PresidentialEscort.pluginManager.CommandManager.CallCommand(null, "SETCONFIG", new string[] {"friendly_fire","false"});
        }

        public IEnumerable<float> SpawnVIP(Player player)
        {
            PresidentialEscort.vip = player;
            Vector spawn = PresidentialEscort.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
            player.ChangeRole(Role.SCIENTIST, false, false, true, false);
            yield return 2;
            player.Teleport(spawn);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are the <color=#f8ea56>VIP</color> Escape the facility with the help of " +
                "<color=#308ADA>NTF</color> while avoiding the <color=#e83e25>SCPs</color>.", false);

        }

        public IEnumerable<float> SpawnNTF(Player player)
        {
            Vector spawn = PresidentialEscort.Server.Map.GetRandomSpawnPoint(Role.CLASSD);
            player.ChangeRole(Role.NTF_CADET, false, true, false, false);
            yield return 2;
            player.Teleport(spawn);

            foreach (Smod2.API.Item item in player.GetInventory())
            {
                item.Remove();
            }
            
            player.SetAmmo(AmmoType.DROPPED_5, 500);
            player.SetAmmo(AmmoType.DROPPED_7, 500);
            player.SetAmmo(AmmoType.DROPPED_9, 500);
            player.GiveItem(ItemType.RADIO);
            player.GiveItem(ItemType.E11_STANDARD_RIFLE);
            player.GiveItem(ItemType.FLASHBANG);
            player.GiveItem(ItemType.MEDKIT);
            player.GiveItem(ItemType.SENIOR_GUARD_KEYCARD);
            player.GiveItem(ItemType.FRAG_GRENADE);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are an <color=#308ADA>NTF Cadet</color>. Work with others to help the " +
                "<color=#f8ea56>VIP</color> escape and eliminate the <color=#e83e25>SCPs</color>", false);
        }
    }
}