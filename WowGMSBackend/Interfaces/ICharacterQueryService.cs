using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface ICharacterQueryService
    {
        List<Character> GetCharactersByMemberId(int memberid);
    }
}
