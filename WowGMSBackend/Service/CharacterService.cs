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
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepo _repo;

        public CharacterService(ICharacterRepo repo)
        {
            _repo = repo;

        }

        public void CreateCharacterWithKills(Character character, Dictionary<string, int> killInputs, int memberId)
        {
            character.MemberId = memberId;
            character.BossKills ??= new List<BossKill>();

            foreach (var kvp in killInputs)
            {
                var bossSlug = kvp.Key;
                var killCount = kvp.Value;

                character.BossKills.Add(new BossKill
                {
                    BossSlug = bossSlug,
                    KillCount = killCount,
                    Character = character
                });
            }

            _repo.AddCharacter(character);
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
            var existing = _repo.GetCharacter(id);
            if (existing == null) return null;

            existing.CharacterName = updated.CharacterName;
            existing.RealmName = updated.RealmName;
            existing.Class = updated.Class;
            existing.Role = updated.Role;
            existing.MemberId = updated.MemberId;
            existing.Member = updated.Member;

            // ✅ DO NOT touch BossKills here

            return _repo.SaveChangesAndReturn(existing);
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