using Smod2;
using Smod2.API;
using System.Collections.Generic;


namespace SurvivalGamemode
{
	public class Functions
    {
		public static Functions singleton;
		public Survival Survival;
		public Functions(Survival plugin)
		{
			this.Survival = plugin;
			Functions.singleton = this;
		}
        public void EnableGamemode()
        {
            Survival.enabled = true;
            if (!Survival.roundstarted)
            {
                Survival.Server.Map.ClearBroadcasts();
                Survival.Server.Map.Broadcast(25, "<color=#50c878>Survival of the Fittest Gamemode</color> is starting..", false);
            }
        }
        public void DisableGamemode()
        {
            Survival.enabled = false;
            Survival.Server.Map.ClearBroadcasts();
        }
        public void EndGamemodeRound()
        {
            Survival.Info("EndgameRound Function");
            Survival.roundstarted = false;
            Survival.Server.Round.EndRound();
            Survival.Info("Toggling Blackout off.");
            if (Survival.blackouts)
            {
               Survival.Info("Enabling timed Blackouts.");
                SCP575.Functions.singleton.EnableBlackouts();
            }
        }

        public void SpawnDboi(Player player)
        {
			Vector spawn;
			if (Survival.zone == "lcz")
			{
				spawn = Survival.Server.Map.GetRandomSpawnPoint(Role.SCIENTIST);
			}
			else
			{
				spawn = Survival.Server.Map.GetRandomSpawnPoint(Role.SCP_096);
			}
            player.ChangeRole(Role.CLASSD, false, false, false, true);
            player.Teleport(spawn);

            foreach (Item item in player.GetInventory())
            {
                item.Remove();
            }

            player.GiveItem(ItemType.FLASHLIGHT);
            player.GiveItem(ItemType.CUP);

            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Find a hiding place and survive from the peanuts! They will spawn in 939's area when the lights go off!", false);
        }

        public void SpawnNut(Player player)
        {

            player.ChangeRole(Role.SCP_173, false, true, true, true);
            Survival.Info("Spawned " + player.Name + " as SCP-173");
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(45, "You will be teleported into the game arena when adequate time has passed for other players to hide...", false);
        }
        public Vector NutSpawn()
        {
            List<Room> rooms = new List<Room>();
			if (Survival.zone == "lcz")
			{
				foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
				{
					if (room.ZoneType == ZoneType.LCZ && room.RoomType != RoomType.CHECKPOINT_A && room.RoomType != RoomType.CHECKPOINT_B && room.RoomType != RoomType.ENTRANCE_CHECKPOINT)
					{
						rooms.Add(room);
					}
				}
			}
			else
			{
            	foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
            	{
	                if (room.ZoneType == ZoneType.HCZ && room.RoomType != RoomType.ENTRANCE_CHECKPOINT && room.RoomType != RoomType.CHECKPOINT_A && room.RoomType != RoomType.CHECKPOINT_B)
                	{
	                    rooms.Add(room);
                	}
            	}
			}
            int randomNum = Survival.gen.Next(rooms.Count);
            Room randomRoom = rooms[randomNum];
            Vector spawn = randomRoom.Position;
            return spawn;
        }
        public IEnumerable<float> TeleportNuts(float delay)
        {
            yield return delay;
            Survival.Info("Timer completed!");
            SCP575.Functions.singleton.ToggleBlackout();
            foreach (Player player in Survival.Server.GetPlayers())
            {
                if (player.TeamRole.Role == Role.SCP_173)
                {
                    player.Teleport(NutSpawn());
                    player.SetHealth(Survival.nut_health);
                    player.PersonalBroadcast(15, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the Class-D before the auto-nuke goes off!", false);
                }
            }
        }
    }
}