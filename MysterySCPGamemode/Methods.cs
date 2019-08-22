using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace SCP
{
	public class Methods
	{
		private readonly Scp plugin;
		public Methods(Scp plugin) => this.plugin = plugin;

		public bool IsAllowed(ICommandSender sender)
		{
			Player player = (Player)sender;

			if (player == null) return true;
			
			List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0 ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

			if (roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
				return true;
			
			return roleList.Count == 0;
		}
		
		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(15, "<color=#c50000>Mystery SCP</color> gamemode is starting..", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}

		public void EndGamemodeRound()
		{
			plugin.Info("Mystery SCP gamemode ended.");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}

		public Role ScpType()
		{
			List<Role> roles = new List<Role>();

			switch (plugin.ScpType)
			{
				case "random":
					roles.Add(Role.SCP_049);
					roles.Add(Role.SCP_106);
					roles.Add(Role.SCP_173);
					roles.Add(Role.SCP_939_53);

					int r = plugin.Gen.Next(1, roles.Count);

					return roles[r];
				case "939":
					return Role.SCP_939_53;
				case "173":
					return Role.SCP_173;
				case "106":
					return Role.SCP_106;
				case "049":
					return Role.SCP_049;
				default:
					return Role.SCP_939_89;
			}
		}

		public IEnumerator<float> SpawnScp(Player player)
		{
			yield return Timing.WaitForOneFrame;

			player.ChangeRole(plugin.Role);
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are an <color=#c50000>SCP</color>. All other SCP's are the same type as you.", false);
		}
	}
}