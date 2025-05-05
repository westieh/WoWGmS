using WoW.Model;

namespace WoWGMS.Service
{
    public interface IBossKillService
    {
        void ApplyKillIfReady(BossRoster roster, DateTime currentTime);
    }
}
