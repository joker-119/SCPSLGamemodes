using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Smod2.API;
using MEC;
using Smod2;
using Smod2.Commands;

namespace SCPRouletteGamemode
{
	public class Methods
	{
		private readonly ScpRouletteGamemode plugin;
		public Methods(ScpRouletteGamemode plugin) => this.plugin = plugin;
		
		public bool IsAllowed(ICommandSender sender)
		{
			Player player = sender as Player;

			if (player != null)
			{
				List<string> roleList = (plugin.ValidRanks != null && plugin.ValidRanks.Length > 0) ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

				if (roleList != null && roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
					return true;
				else if (roleList == null || roleList.Count == 0)
					return true;
				else
					return false;
			}
			return true;
		}

		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(10, "<color=#760101>SCP Roulette</color> is starting..", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}
		

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