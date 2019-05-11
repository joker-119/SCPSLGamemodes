using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Smod2.API;
using MEC;
using Smod2;

namespace SCPRouletteGamemode
{
	public class Methods
	{
		private readonly ScpRouletteGamemode plugin;
		public Methods(ScpRouletteGamemode plugin) => this.plugin = plugin;

		public void PickScp(Player player)
		{
			while (true)
			{
				bool scp096 = false;
				int r = plugin.Gen.Next(plugin.ScpRoles.Count);

				foreach (Player ply in plugin.Server.GetPlayers().Where(ply => ply.TeamRole.Team == Smod2.API.Team.SCP))
					if (ply.TeamRole.Role == Role.SCP_096)
						scp096 = true;

				if (plugin.ScpRoles[r] == Role.SCP_096 && scp096) continue;

				Timing.RunCoroutine(ChangeScp(player, plugin.ScpRoles[r]));
				break;
			}
		}

		private IEnumerator<float> ChangeScp(Player player, Role role)
		{
			yield return Timing.WaitForSeconds(plugin.Delay);
			
			player.ChangeRole(role, false, false, false);
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(5, "You have killed someone and as a result, your SCP role has changed.", false);
		}
	}
}