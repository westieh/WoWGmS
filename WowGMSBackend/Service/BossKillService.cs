using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public class BossKillService : IBossKillService
    {
        private readonly IBossKillRepo _bossKillRepo;

        public BossKillService(IBossKillRepo bossKillRepo)
        {
            _bossKillRepo = bossKillRepo;
        }
        public BossKill? GetMostKilledBossForCharacter(int characterId)
        {
            return _bossKillRepo
                .GetBossKillsByCharacterId(characterId)
                .GroupBy(k => k.BossSlug)
                .Select(g => new
                {
                    BossSlug = g.Key,
                    Count = g.Count(),
                    ExampleKill = g.First()  // to preserve full BossKill reference
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault()?.ExampleKill;
        }

        public void SetBossKillsForCharacter(int characterId, List<BossKill> kills)
        {
            _bossKillRepo.DeleteBossKillsForCharacter(characterId);
            foreach (var kill in kills)
            {
                kill.CharacterId = characterId;
                _bossKillRepo.AddBossKill(kill);
            }
        }

        public List<BossKill> GetBossKillsForCharacter(int characterId)
        {
            return _bossKillRepo.GetBossKillsByCharacterId(characterId);
        }

        public void IncrementBossKill(int characterId, string bossSlug)
        {
            var kills = _bossKillRepo.GetBossKillsByCharacterId(characterId);
            var existing = kills.Find(k => k.BossSlug == bossSlug);
            if (existing != null)
            {
                existing.KillCount++;
                _bossKillRepo.AddBossKill(existing);
            }
            else
            {
                _bossKillRepo.AddBossKill(new BossKill
                {
                    BossSlug = bossSlug,
                    KillCount = 1,
                    CharacterId = characterId
                });
            }
        }

        public void TransferFromApplication(Application application, int characterId)
        {
            var kills = application.BossKills?.Select(bk => new BossKill
            {
                BossSlug = bk.BossSlug,
                KillCount = bk.KillCount,
                CharacterId = characterId
            }).ToList() ?? new List<BossKill>();

            SetBossKillsForCharacter(characterId, kills);
        }
    }
}