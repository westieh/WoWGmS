using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Registry;
using WowGMSBackend.ViewModels;
namespace WowGMSBackend.Service
{
    public class BossKillService : IBossKillService
    {
        private readonly IBossKillRepo _bossKillRepo;
        private readonly ICharacterQueryService _characterQueryService;

        public BossKillService(IBossKillRepo bossKillRepo, ICharacterQueryService characterQueryService)
        {
            _bossKillRepo = bossKillRepo;
            _characterQueryService = characterQueryService;
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

        public MemberPanelViewModel GetPanelData(int memberId, string selectedBossSlug)
        {
            var allBosses = (RaidRegistry.Raids ?? new List<Raid>())
                .Where(r => r?.Bosses != null)
                .SelectMany(r => r.Bosses)
                .Where(b => !string.IsNullOrEmpty(b.Slug) && !string.IsNullOrEmpty(b.DisplayName))
                .ToList();

            var characters = _characterQueryService.GetCharactersByMemberId(memberId);

            return new MemberPanelViewModel
            {
                AllBosses = allBosses,
                Characters = characters,
                SelectedBossSlug = selectedBossSlug
            };
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