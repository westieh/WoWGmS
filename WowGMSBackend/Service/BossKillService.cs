using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Registry;
using WowGMSBackend.Repository;

namespace WowGMSBackend.Service
{
    public class BossKillService : IBossKillService
    {
        private readonly IBossKillRepo _bossKillRepo;
        private readonly ICharacterService _characterService;


        public BossKillService(IBossKillRepo bossKillRepo, ICharacterService characterService)
        {
            _bossKillRepo = bossKillRepo;
            _characterService = characterService;

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
                    ExampleKill = g.First()
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
        public void SetOrUpdateSingleBossKill(int characterId, string bossSlug, int newKillCount)
        {
            var character = _characterService.GetCharacter(characterId);
            if (character == null) return;

            character.BossKills ??= new List<BossKill>();

            var existingKill = character.BossKills.FirstOrDefault(k => k.BossSlug == bossSlug);

            if (existingKill != null)
            {
                existingKill.KillCount = newKillCount;
            }
            else
            {
                character.BossKills.Add(new BossKill
                {
                    BossSlug = bossSlug,
                    KillCount = newKillCount,
                    CharacterId = characterId
                });
            }

            _characterService.UpdateCharacter(characterId, character); 
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

        public List<Boss> GetAllBosses()
        {
            return (RaidRegistry.Raids ?? new List<Raid>())
                .Where(r => r?.Bosses != null)
                .SelectMany(r => r.Bosses)
                .Where(b => !string.IsNullOrEmpty(b.Slug) && !string.IsNullOrEmpty(b.DisplayName))
                .ToList();
        }
        public Dictionary<int, int> GetBossKillCountsForRoster(BossRoster roster)
        {
            var result = new Dictionary<int, int>();

            foreach (var character in roster.Participants)
            {
                var kills = _bossKillRepo.GetBossKillsByCharacterId(character.Id);
                var total = kills
                    .Where(k => k.BossSlug == roster.BossSlug)
                    .Sum(k => k.KillCount);

                result[character.Id] = total;
            }

            return result;
        }
    }
}