using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace MassacreGamemode
{
    public class Functions
    {
        public static Functions singleton;
        public Massacre Massacre;
        public Functions(Massacre plugin)
        {
            this.Massacre = plugin;
            Functions.singleton = this;
        }
        public void EnableGamemode()
        {
            Massacre.enabled = true;
            if (!Massacre.roundstarted)
            {
                Massacre.pluginManager.Server.Map.ClearBroadcasts();
                Massacre.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Massacre of the D-Bois Gamemode</color> is starting..", false);
            }
        }
        public void DisableGamemode()
        {
            Massacre.enabled = false;
            Massacre.pluginManager.Server.Map.ClearBroadcasts();
        }
        public Vector SpawnLoc()
        {
            Vector spawn = null;

            switch (Massacre.SpawnRoom.ToLower())
            {
                case "jail":
                    {
                        Massacre.Info("Jail room selected.");
                        spawn = new Vector(53, 1020, -44);
                        return spawn;
                    }
                case "939":
                    {
                        Massacre.Info("939 Spawn Room selected");
                        spawn = Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);
                        return spawn;
                    }
                case "049":
                    {
                        Massacre.Info("049 Spawn room selected");
                        spawn = Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_049);
                        return spawn;
                    }
                case "106":
                    {
                        Massacre.Info("106 Spawn room selected");
                        spawn = Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_106);
                        return spawn;
                    }
                case "173":
                    {
                        Massacre.Info("173 Spawn room selected.");
                        spawn = Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_173);
                        return spawn;
                    }
                case "random":
                    {
                        Massacre.SpawnLocs.Add(Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53));
                        Massacre.SpawnLocs.Add(Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_173));
                        Massacre.SpawnLocs.Add(Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_049));
                        Massacre.SpawnLocs.Add(Massacre.Server.Map.GetRandomSpawnPoint(Role.SCP_106));
                        Massacre.SpawnLocs.Add(new Vector(53, 1020, -44));
                        int RandomInt = new System.Random().Next(Massacre.SpawnLocs.Count);
                        return Massacre.SpawnLocs[RandomInt];
                    }
                default:
                    {
                        Massacre.Info("Invalid location selected, defaulting to Jail.");
                        spawn = new Vector(53, 1020, -44);
                        return spawn;
                    }
            }
        }
        public IEnumerable<float> SpawnDboi(Player player, float delay)
        {
            player.ChangeRole(Role.CLASSD, false, false, false, true);
            player.Teleport(Massacre.SpawnLoc);
            yield return 2;

            foreach (Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.CUP);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Get ready to die!", false);
        }
        public IEnumerable<float> SpawnNut(Player player, float delay)
        {
            player.ChangeRole(Role.SCP_173, false, false, false, false);
            yield return 5.5f;
            player.Teleport(Massacre.SpawnLoc);
            Massacre.Info("Spawned " + player.Name + " as SCP-173");
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(35, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the D-bois!", false);
            player.SetHealth(Massacre.nut_health);
        }
        public void EndGamemodeRound()
        {
            Massacre.Info("EndgameRound Function");
            Massacre.roundstarted = false;
            Massacre.Server.Round.EndRound();
        }
    }
}