using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface ICharacterRepo
    {
        Character AddCharacter(Character character);
        Character? GetCharacter(int id);
        List<Character> GetCharacters();
        Character? UpdateCharacter(int id, Character updated);
        Character? DeleteCharacter(int id);
        List<Character> GetCharactersByMemberId(int memberId);
        List<Character> GetCharactersByRoster(int rosterId);
        void AddBossKill(BossKill bossKill);
        Character SaveChangesAndReturn(Character character);

    }
}
