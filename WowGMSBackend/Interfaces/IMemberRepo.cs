using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface IMemberRepo
    {
        List<Member> GetMembers();
        Member? GetMember(int id);
        Member AddMember(Member member);
        Member? UpdateMember(int id, Member member);
        Member? DeleteMember(int id);
    }
}
