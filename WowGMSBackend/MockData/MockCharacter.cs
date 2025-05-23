using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Model;

namespace WowGMSBackend.MockData
{
    public class MockCharacter
    {
        public static List<Character> GetMockCharacters(List<Member> members)
        {
            return new List<Character>
    {
        new Character("Arthas", Class.Warrior, Role.Tank, ServerName.Arthas)
        {
            Id = 1,
            MemberId = members[0].MemberId,
            Member = members[0]
        },
        new Character("Jaina", Class.Mage, Role.RangedDPS, ServerName.Aegwynn) // Assuming Kul Tiras is not in enum, use closest or add
        {
            Id = 2,
            MemberId = members[1].MemberId,
            Member = members[1]
        },
        new Character("Anduin", Class.Priest, Role.Healer, ServerName.Stormrage) // Using Stormrage as example
        {
            Id = 3,
            MemberId = members[2].MemberId,
            Member = members[2]
        }
    };
        }
    }
}
