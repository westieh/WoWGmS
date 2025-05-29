using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;
using WowGMSBackend.ViewModels;

namespace WowGMSBackend.Interfaces
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
        Dictionary<int, List<CharacterWithKill>> GetGroupedCharactersByBossSlug(string bossSlug);
        List<Character> GetAllCharactersWithMemberAndBossKills();
        Character CreateCharacterWithKills(Character character, Dictionary<string, int> killInputs, int memberId);

        void AddBossKill(BossKill bossKill);

    }
}