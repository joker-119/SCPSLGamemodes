using Smod2;
using Smod2.API;
using Smod2.Commands;
using System.Linq;
using System.Collections.Generic;
using MEC;

namespace SCP
{
	public class Methods
	{
		private readonly SCP plugin;
		public Methods(SCP plugin) => this.plugin = plugin;

		public bool IsAllowed(ICommandSender sender)
		{
			Player player = (Player)sender;

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
			if (plugin.RoundStarted) return;

			plugin.Server.Map.Broadcast(10, "<color=#c50000>Mystery SCP</color> gamemode is starting..", false);
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

		public Role SCPType()
		{
			List<Role> roles = new List<Role>();

			switch (plugin.SCPType)
			{
				case "random":
					roles.Add(Role.SCP_049);
					roles.Add(Role.SCP_096);
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
				case "096":
					return Role.SCP_096;
				case "049":
					return Role.SCP_049;

			}
			return Role.SCP_939_89;
		}

		public IEnumerator<float> SpawnSCP(Player player)
		{
			yield return Timing.WaitForOneFrame;

			player.ChangeRole(plugin.role);
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(15, "You are an <color=#c50000>SCP</color>. All other SCP's are the same type as you.", false);
		}
	}
}