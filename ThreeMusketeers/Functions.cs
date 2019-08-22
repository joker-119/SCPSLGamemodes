using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace ThreeMusketeers
{
	public class Functions
	{
		private readonly Musketeers plugin;

		public Functions(Musketeers plugin) => this.plugin = plugin;

		public bool IsAllowed(ICommandSender sender)
		{
			if (!(sender is Player player)) return true;
			
			List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0 ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

			if (roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
				return true;
			return roleList.Count == 0;
		}
		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (plugin.RoundStarted) return;
			
			plugin.Server.Map.ClearBroadcasts();
			plugin.Server.Map.Broadcast(25, "<color=#308ADA> Three Musketeers</color> gamemode starting..", false);
		}
		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}
		public IEnumerator<float> SpawnNtf(Player player)
		{
			player.ChangeRole(Role.NTF_COMMANDER, true, true, false, true);
			yield return Timing.WaitForSeconds(2);

			player.SetHealth(plugin.NtfHealth);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are a <color=#308ADA>Musketeer</color>. Enter the facility and eliminate all Class-D.", false);
		}
		public IEnumerator<float> SpawnClassD(Player player)
		{
			player.ChangeRole(Role.CLASSD, true, true, false, true);
			yield return Timing.WaitForSeconds(2);

			player.SetHealth(plugin.ClassDHealth);

			player.GiveItem(ItemType.USP);
			player.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are a <color=#DAA130>Class-D personnel</color>. Escape the facility before the auto-nuke, but evade the NTF sent to kill you!", false);
		}
		public void EndGamemodeRound()
		{
			plugin.Info("The Gamemode round has ended!");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}
	}
}