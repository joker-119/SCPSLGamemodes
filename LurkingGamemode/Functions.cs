using Smod2;
using Smod2.API;

namespace LurkingGamemode
{
    public class Functions
    {
        public static Functions singleton;
        public Lurking Lurking;
        public Functions(Lurking plugin)
        {
            this.Lurking = plugin;
            Functions.singleton = this;
        }
        public void EndGamemodeRound()
        {
            Lurking.Info("EndgameRound Function");
            Lurking.roundstarted = false;
            Lurking.Server.Round.EndRound();

            if (Lurking.blackouts)
            {
                SCP575.Functions.singleton.EnableBlackouts();
                Lurking.Info("Enabling timed Blackouts.");
            }
        }

        public void SpawnLarry(Player player)
        {
            player.ChangeRole(Role.SCP_106, false, true, false, false);
            player.SetHealth(Lurking.larry_health);
            player.PersonalClearBroadcasts();
            player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
        }
        public void SpawnDoggo(Player player)
        {
            if (player.TeamRole.Role != Role.SCP_106)
            {
                player.ChangeRole(Role.SCP_939_53, false, true, false, false);
                player.SetHealth(Lurking.doggo_health);
                player.PersonalClearBroadcasts();
                player.PersonalBroadcast(25, "You are <color=#2D2B2B> what lurks in the dark</color>, your job is to kill the Scientists before they escape.", false);
            }
        }
        public void EnableGamemode()
        {
            Lurking.enabled = true;
            if (!Lurking.roundstarted)
            {
                Lurking.Server.Map.ClearBroadcasts();
                Lurking.Server.Map.Broadcast(25, "<color=#2D2B2B> Lurking in the Dark</color> Gamemode starting..", false);
            }
        }

        public void DisableGamemode()
        {
            Lurking.enabled = false;
            Lurking.Server.Map.ClearBroadcasts();
        }
    }
}