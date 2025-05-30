using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Registry;
using WowGMSBackend.Repository;

namespace WowGMSBackend.Service
{
    /// <summary>
    /// Service responsible for handling boss kill related operations
    /// for characters and applications.
    /// Provides functionalities for managing boss kill records.
    /// </summary>
    public class BossKillService : IBossKillService
    {
        private readonly IBossKillRepo _bossKillRepo;
        private readonly ICharacterService _characterService;

        /// <summary>
        /// Initializes the BossKillService with required dependencies.
        /// </summary>
        public BossKillService(IBossKillRepo bossKillRepo, ICharacterService characterService)
        {
            _bossKillRepo = bossKillRepo;
            _characterService = characterService;
        }

        /// <summary>
        /// Returns the boss most frequently killed by a given character.
        /// Groups boss kills by boss slug and selects the one with the highest count.
        /// </summary>
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

        /// <summary>
        /// Clears all existing boss kills for a character and sets a new list.
        /// Existing records are deleted before new ones are inserted.
        /// </summary>
        public void SetBossKillsForCharacter(int characterId, List<BossKill> kills)
        {
            _bossKillRepo.DeleteBossKillsForCharacter(characterId);
            foreach (var kill in kills)
            {
                kill.CharacterId = characterId;
                _bossKillRepo.AddBossKill(kill);
            }
        }

        /// <summary>
        /// Retrieves all boss kill records for a given character.
        /// </summary>
        public List<BossKill> GetBossKillsForCharacter(int characterId)
        {
            return _bossKillRepo.GetBossKillsByCharacterId(characterId);
        }

        /// <summary>
        /// Sets or updates the kill count for a specific boss for a character.
        /// If the boss kill exists, updates the kill count; otherwise, creates a new record.
        /// </summary>
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

        /// <summary>
        /// Increments the boss kill count by one for a given boss for a character.
        /// If the boss kill does not exist, creates it with a kill count of 1.
        /// </summary>
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

        /// <summary>
        /// Transfers boss kill data from an application to a character.
        /// Used when approving an application and transferring its associated boss kills.
        /// </summary>
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

        /// <summary>
        /// Retrieves a list of all bosses across all raids.
        /// Used for lookup and validation purposes.
        /// </summary>
        public List<Boss> GetAllBosses()
        {
            return (RaidRegistry.Raids ?? new List<Raid>())
                .Where(r => r?.Bosses != null)
                .SelectMany(r => r.Bosses)
                .Where(b => !string.IsNullOrEmpty(b.Slug) && !string.IsNullOrEmpty(b.DisplayName))
                .ToList();
        }

        /// <summary>
        /// Computes total boss kill counts for each character in a given roster.
        /// The count is aggregated per character based on the boss slug associated with the roster.
        /// </summary>
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
