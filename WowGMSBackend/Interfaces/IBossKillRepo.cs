using System.Collections.Generic;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface IBossKillRepo
    {
        void AddBossKill(BossKill bossKill);
        void DeleteBossKillsForCharacter(int characterId);
        List<BossKill> GetBossKillsByCharacterId(int characterId);
    }
}