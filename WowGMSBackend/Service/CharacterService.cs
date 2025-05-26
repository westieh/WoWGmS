using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;

namespace WowGMSBackend.Service
{
   public class CharacterService : ICharacterService
    {
        private readonly CharacterRepo _repo;

        public CharacterService(CharacterRepo repo)
        {
            _repo = repo;
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
            return _repo.GetAllCharacters();
        }
    }
}