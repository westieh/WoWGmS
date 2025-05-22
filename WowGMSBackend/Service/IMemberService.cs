using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public interface IMemberService
    {
        Member AddMember(Member member);
        Member? GetMember(int memberId);
        Member? UpdateMember(int memberId, Member member);
        Member? DeleteMember(int memberId);
        List<Member> GetMembers();
        Member? ChangeMemberRank(int actingMemberId, int targetMemberId, Rank newRank);
    }
}
