using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace MuskateersGamemode
{
	public class Functions
    {
		public static Functions singleton;
		public Muskateers Muskateers;
		public Functions(Muskateers plugin)
		{
			this.Muskateers = plugin;
			Functions.singleton = this;
		}
        public void EnableGamemode()
        {
            Muskateers.enabled = true;
            if (!Muskateers.roundstarted)
            {
                Muskateers.pluginManager.Server.Map.ClearBroadcasts();
                Muskateers.pluginManager.Server.Map.Broadcast(25, "<color=#308ADA> Three Muskateers</color> gamemode starting..", false);
            }
        }
        public void DisableGamemode()
        {
            Muskateers.enabled = false;
            Muskateers.pluginManager.Server.Map.ClearBroadcasts();
        }
        public IEnumerable<float> SpawnNTF(Player player)
        {
            player.ChangeRole(Role.NTF_COMMANDER, true, true, false, true);
			yield return 2;
            player.SetHealth(Muskateers.ntf_health);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#308ADA>Muskateer</color>. Enter the facility and eliminate all Class-D.", false);
        }
        public IEnumerable<float> SpawnClassD(Player player)
        {
            player.ChangeRole(Role.CLASSD, true, true, false, true);
			yield return 2;
            player.SetHealth(Muskateers.classd_health);
			player.GiveItem(ItemType.USP);
			player.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#DAA130>Class-D personnel</color>. Escape the facility before the auto-nuke, but evade the NTF sent to kill you!", false);
        }
        public void EndGamemodeRound()
        {
            Muskateers.Info("The Gamemode round has ended!");
            Muskateers.roundstarted = false;
            Muskateers.Server.Round.EndRound();
        }
    }
}