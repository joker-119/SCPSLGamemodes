using System.Diagnostics.Contracts;
using System.Security;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using System;
using System.Linq;
using scp4aiur;
using System.Collections.Generic;

namespace Mystery
{
    public class Functions
    {
        private readonly Mystery plugin;

        public Functions(Mystery plugin) => this.plugin = plugin;

        public bool IsAllowed(ICommandSender sender)
        {
            Player player = (sender is Player) ? sender as Player : null;

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

        public void DisableGamemode()
        {
            plugin.Enabled = false;
            plugin.pluginManager.Server.Map.ClearBroadcasts();
        }

        public void EnableGamemode()
        {
            plugin.Enabled = true;

            if (!plugin.RoundStarted)
            {
                plugin.Server.Map.ClearBroadcasts();
                plugin.Server.Map.Broadcast(25, "<color=#c50000>Murder Mystery</color> gamemode is starting..", false);
            }
        }

        public void EndGamemoderound()
        {
            plugin.Info("Endgame function.");
            plugin.RoundStarted = false;
            plugin.Server.Round.EndRound();
            plugin.pluginManager.CommandManager.CallCommand(null, "SETCONFIG", new string[] { "friendly_fire", "false" });
        }

        public IEnumerable<float> SpawnMurd(Player player)
        {
            Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.CLASSD);

            player.ChangeRole(Role.CLASSD, false, false, false, false);

            yield return 1;

            player.Teleport(spawn);

            foreach (Smod2.API.Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.USP);
            player.GiveItem(ItemType.MEDKIT);
            player.GiveItem(ItemType.ZONE_MANAGER_KEYCARD);
            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.COIN);
            player.GiveItem(ItemType.RADIO);

            player.SetAmmo(AmmoType.DROPPED_5, 500);
            player.SetAmmo(AmmoType.DROPPED_7, 500);
            player.SetAmmo(AmmoType.DROPPED_9, 500);

            player.SetHealth(plugin.MurdHealth);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are a <color=#c50000> Murderer</color>. You must murder all of the Civilians before the detectives find and kill you.", false);
        }
        public IEnumerable<float> SpawnDet(Player player)
        {
            Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCIENTIST);

            player.ChangeRole(Role.SCIENTIST, false, false, false, false);

            yield return 1;

            player.Teleport(spawn);

            foreach (Smod2.API.Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.COM15);
            player.GiveItem(ItemType.CONTAINMENT_ENGINEER_KEYCARD);
            player.GiveItem(ItemType.MEDKIT);
            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.DISARMER);

            player.SetHealth(plugin.DetHealth);

            player.SetAmmo(AmmoType.DROPPED_9, 500);

            plugin.murd.Add(player.SteamId, true);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are a <color=#DAD530> Detective</color>. You must find all of the Murderers before they kill all of the Civilians!", false);
        }
        public IEnumerable<float> SpawnCiv(Player player)
        {
            Vector spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.CLASSD);

            player.ChangeRole(Role.CLASSD, false, false, false, false);

            yield return 1;

            player.Teleport(spawn);

            foreach (Smod2.API.Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.JANITOR_KEYCARD);
            player.GiveItem(ItemType.COIN);
            player.GiveItem(ItemType.CUP);

            plugin.murd.Add(player.SteamId, false);

            player.SetHealth(plugin.MurdHealth);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are a <color=#5AD3D9>Civilian</color>. You must help the Detectives find the murderers, before they kill all of your friends!", false);
        }
    }
}