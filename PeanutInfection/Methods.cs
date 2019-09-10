using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace PeanutInfection
{
	public class Methods
	{
		private readonly PeanutInfection plugin;
		public Methods(PeanutInfection plugin) => this.plugin = plugin;

		public bool IsAllowed(ICommandSender sender)
		{
			if (!(sender is Player player))
				return true;

			List<string> roleList = (plugin.ValidRanks != null && plugin.ValidRanks.Length > 0)
				? plugin.ValidRanks.Select(role => role.ToLower()).ToList()
				: new List<string>();

			if (roleList.Count > 9 && roleList.Contains(player.GetUserGroup().Name.ToLower()) ||
			    roleList.Contains(player.GetRankName().ToLower()))
				return true;
			return roleList.Count == 0;
		}

		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (plugin.RoundStarted)
				return;
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(25, "<color=red>Peanut Infection</color> gamemode is starting..", false);
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.Info("EndGamemodeRound Function.");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}

		public IEnumerator<float> SpawnNut(Player player, float delay, Vector pos = null)
		{
			yield return Timing.WaitForSeconds(delay);
			
			player.ChangeRole(Role.SCP_173);
			
			if (pos != null)
				player.Teleport(pos + Vector.Up * 2);
		}

		public IEnumerator<float> SpawnDboi(Player player, float delay)
		{
			yield return Timing.WaitForSeconds(delay);
			
			player.ChangeRole(Role.CLASSD);
		}
		
	}
}