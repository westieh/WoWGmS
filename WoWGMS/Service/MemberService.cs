using WoW.Model;
using WoWGMS.Repository;

namespace WoWGMS.Service
{
    public class MemberService : IMemberService
    {
        private readonly MemberRepo _memberRepo;

        public MemberService(MemberRepo memberRepo)
        {
            _memberRepo = memberRepo;
        }

        public Member AddMember(Member member)
        {
            if (string.IsNullOrWhiteSpace(member.Name))
                throw new ArgumentException("Name cannot be empty");

            return _memberRepo.AddMember(member);
        }

        public Member? GetMember(int memberId)
        {
            return _memberRepo.GetMember(memberId);
        }

        public List<Member> GetMembers()
        {
            return _memberRepo.GetMembers();
        }

        public Member? UpdateMember(int memberId, Member updatedMember)
        {
            var existing = _memberRepo.GetMember(memberId);
            if (existing == null) return null;

            return _memberRepo.UpdateMember(memberId, updatedMember);
        }

        public Member? DeleteMember(int memberId)
        {
            var member = _memberRepo.GetMember(memberId);
            if (member == null) return null;

            if (member.Rank == Rank.Officer)
                throw new InvalidOperationException("Cannot delete an officer");

            return _memberRepo.DeleteMember(memberId);
        }

        public Member? ChangeMemberRank(int actingMemberId, int targetMemberId, Rank newRank)
        {
            var acting = _memberRepo.GetMember(actingMemberId);
            var target = _memberRepo.GetMember(targetMemberId);

            if (acting == null || target == null)
                return null;

            if (acting.Rank != Rank.Officer)
                throw new UnauthorizedAccessException("Only officers can change ranks.");

            target.Rank = newRank;
            return _memberRepo.UpdateMember(target.MemberId, target);
        }
    }
}