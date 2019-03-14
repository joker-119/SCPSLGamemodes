using Smod2;
using UnityEngine;
using Smod2.API;
using System.Collections.Generic;


namespace ZombielandGamemode
{
	public class Functions
    {
		public static Functions singleton;
		public Zombieland Zombieland;
		public Functions(Zombieland plugin)
		{
			this.Zombieland = plugin;
			Functions.singleton = this;
		}
        public void EnableGamemode()
        {
            Zombieland.enabled = true;
            if (!Zombieland.roundstarted)
            {
                Zombieland.pluginManager.Server.Map.ClearBroadcasts();
                Zombieland.pluginManager.Server.Map.Broadcast(25, "<color=#50c878>Zombieland Gamemode</color> is starting..", false);
            }
        }
        public void DisableGamemode()
        {
            Zombieland.enabled = false;
            Zombieland.pluginManager.Server.Map.ClearBroadcasts();
        }
        public void EndGamemodeRound()
        {
            Zombieland.Info("EndgameRound Function");
            Zombieland.roundstarted = false;
            Zombieland.Server.Round.EndRound();
			Zombieland.pluginManager.CommandManager.CallCommand(null, "SETCONFIG", new string[] {"friendly_fire","false"});
        }

        public IEnumerable<float> SpawnChild(Player player, Player killer)
        {
            Vector spawn = player.GetPosition();
            player.ChangeRole(Role.SCP_049_2, false, false, false, false);
			yield return 2;
            player.SetHealth(Zombieland.child_health);
            player.Teleport(spawn);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, killer.Name + " killed you, and you became a <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
        }
        public IEnumerable<float> AliveCounter(float delay)
        {
            while (Zombieland.enabled || Zombieland.roundstarted)
            {
                Zombieland.Server.Map.ClearBroadcasts();
                int human_count = (Zombieland.Round.Stats.NTFAlive + Zombieland.Round.Stats.ScientistsAlive + Zombieland.Round.Stats.ClassDAlive + Zombieland.Round.Stats.CiAlive);
                Zombieland.Server.Map.Broadcast(10, "There are currently " + Zombieland.Round.Stats.Zombies + " zombies and " + human_count + " humans alive.", false);
                yield return delay;
            }
        }
        public IEnumerable<float> SpawnAlpha(Player player)
        {
            Vector spawn = Zombieland.Server.Map.GetRandomSpawnPoint(Role.SCP_049);
            player.ChangeRole(Role.SCP_049_2, false, false, false, false);
            yield return 2;
            player.Teleport(spawn);
            Zombieland.Alpha.Add(player);
            player.SetHealth(Zombieland.zombie_health);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(15, "You are an alpha <color=#c50000>Zombie</color>! Attacking or killing humans creates more zombies! Death to the living!", false);
        }
		public IEnumerable<float> OpenGates(float delay)
		{
			yield return delay;
			foreach (Smod2.API.Door door in Zombieland.Server.Map.GetDoors())
			{
				if (door.Name == "GATE_A" || door.Name == "GATE_B")
				{
					door.Open = true;
					door.Locked = true;
				}
			}
		}
    }
}