using System.Security.Claims;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface IMemberService
    {
        Member AddMember(Member member);
        Member? GetMember(int memberId);
        Member? UpdateMember(int memberId, Member member);
        Member? DeleteMember(int memberId);
        List<Member> GetMembers();
        Member? ChangeMemberRank(int actingMemberId, int targetMemberId, Rank newRank);
        Member? ValidateLogin(string name, string password);
        Member? GetMemberByName(string name);
        int? GetLoggedInMemberId(ClaimsPrincipal user);
    }
}
