using System.Collections.Generic;
using WowGMSBackend.Model;
using WowGMSBackend.ViewModels;

namespace WowGMSBackend.Interfaces
{
    public interface IBossKillService
    {
        void SetBossKillsForCharacter(int characterId, List<BossKill> kills);
        List<BossKill> GetBossKillsForCharacter(int characterId);
        BossKill? GetMostKilledBossForCharacter(int characterId);
        void IncrementBossKill(int characterId, string bossSlug);
        void TransferFromApplication(Application application, int characterId);
        MemberPanelViewModel GetPanelData(int memberId, string selectedBossSlug);
        Dictionary<int, int> GetBossKillCountsForRoster(BossRoster roster);
    }
}