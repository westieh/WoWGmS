using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
using WowGMSBackend.ViewModels;

namespace WowGMSBackend.Service
{
    public class CharacterService : ICharacterService, ICharacterQueryService
    {
        private readonly ICharacterRepo _repo;
        private readonly IBossKillRepo _bossKillRepo;

        public CharacterService(ICharacterRepo repo)
        {
            _repo = repo;

        }
        public Dictionary<int, List<CharacterWithKill>> GetGroupedCharactersByBossSlug(string bossSlug)
        {
            var allChars = _repo.GetCharacters(); // or any method that loads boss kills
            var withKills = allChars.Select(c =>
            {
                var kills = c.BossKills.Where(k => k.BossSlug == bossSlug).ToList();
                return new CharacterWithKill
                {
                    Character = c,
                    KillCount = kills.Sum(k => k.KillCount)
                };
            });

            return withKills
                .GroupBy(c => c.Character.MemberId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
        public Character CreateCharacterWithKills(Character character, Dictionary<string, int> killInputs, int memberId)
        {
            character.MemberId = memberId;
            var savedCharacter = AddCharacter(character);

            if (savedCharacter == null)
                throw new Exception("Character could not be created.");

            var bossKills = killInputs
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => new BossKill
                {
                    BossSlug = kvp.Key,
                    KillCount = kvp.Value,
                    CharacterId = savedCharacter.Id
                }).ToList();

            _bossKillRepo.DeleteBossKillsForCharacter(savedCharacter.Id);
            foreach (var kill in bossKills)
            {
                _bossKillRepo.AddBossKill(kill);
            }
            return savedCharacter;
        }

        public Character AddCharacter(Character character)
        {
            return _repo.AddCharacter(character);
        }

        public Character? GetCharacter(int id)
        {
            return _repo.GetCharacter(id);
        }

        public List<Character> GetCharacters()
        {
            return _repo.GetCharacters();
        }

        public Character? UpdateCharacter(int id, Character updated)
        {
            return _repo.UpdateCharacter(id, updated);
        }

        public Character? DeleteCharacter(int id)
        {
            return _repo.DeleteCharacter(id);
        }
        public void AddBossKill(BossKill bossKill)
        {
            _repo.AddBossKill(bossKill);
        }
        public void IncrementBossKill(int characterId, string bossSlug)
        {
            var character = _repo.GetCharacter(characterId);
            character?.IncrementBossKill(bossSlug);
        }

        public List<Character> GetCharactersByMemberId(int memberId)
        {
            return _repo.GetCharactersByMemberId(memberId);
        }

        public List<Character> GetAllCharacters()
        {
            return _repo.GetCharacters();
        }
        public List<Character> GetAllCharactersWithMemberAndBossKills()
        {
            return _repo.GetCharacters();
        }
    }
}