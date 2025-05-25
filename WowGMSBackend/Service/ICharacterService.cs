using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public interface ICharacterService
    {
        Character AddCharacter(Character character);
        Character? GetCharacter(int id);
        List<Character> GetCharacters();
        Character? UpdateCharacter(int id, Character updated);
        Character? DeleteCharacter(int id);
        void IncrementBossKill(int characterId, string bossSlug);
        List<Character> GetCharactersByMemberId(int memberId);
    }
}