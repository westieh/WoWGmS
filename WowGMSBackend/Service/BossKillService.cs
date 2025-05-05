using WoW.Model;

namespace WoWGMS.Service
{
    public class BossKillService : IBossKillService
    {
        public void ApplyKillIfReady(BossRoster roster, DateTime currentTime)
        {
            if (roster.IsProcessed || currentTime < roster.InstanceTime)
                return;

            foreach (var character in roster.Participants)
            {
                character.IncrementBossKill(roster.BossName);
            }

            roster.IsProcessed = true;
        }
    }

}
