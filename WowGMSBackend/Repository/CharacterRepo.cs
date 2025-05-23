using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Model;

namespace WowGMSBackend.Repository
{
    public class CharacterRepo
    {
        private readonly List<Character> _characters = new();
        private int _currentId = 0;

        public CharacterRepo(List<Member> members)
        {
            _characters = new List<Character>(MockData.MockCharacter.GetMockCharacters(members));
        }

        public Character AddCharacter(Character character)
        {
            character.Id = ++_currentId;
            _characters.Add(character);
            return character;
        }

        public Character? GetCharacter(int id)
        {
            return _characters.FirstOrDefault(c => c.Id == id);
        }

        public List<Character> GetCharacters()
        {
            return new List<Character>(_characters);
        }

        public Character? UpdateCharacter(int id, Character updated)
        {
            var existing = _characters.FirstOrDefault(c => c.Id == id);
            if (existing != null)
            {
                existing.CharacterName = updated.CharacterName;
                existing.RealmName = updated.RealmName;
                existing.Class = updated.Class;
                existing.Role = updated.Role;
                existing.MemberId = updated.MemberId;
                existing.Member = updated.Member;
                return existing;
            }
            return null;
        }

        public Character? DeleteCharacter(int id)
        {
            var character = _characters.FirstOrDefault(c => c.Id == id);
            if (character != null)
            {
                _characters.Remove(character);
                return character;
            }
            return null;
        }

        public List<Character> GetCharactersByMemberId(int memberId)
        {
            return _characters.Where(c => c.MemberId == memberId).ToList();
        }
    }
}
