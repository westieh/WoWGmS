using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Model;

namespace WowGMSBackend.Repository
{
    public class MemberRepo
    {
        private readonly List<Member> _members = new List<Member>();


        public MemberRepo()
        {
            _members.AddRange(MockData.MockMember.GetMockMembers());
        }

        public Member AddMember(Member member)
        {
            _members.Add(member);
            return member;
        }

        public Member? GetMember(int memberId)
        {
            return _members.FirstOrDefault(m => m.MemberId == memberId);
        }

        public Member? UpdateMember(int memberId, Member updatedMember)
        {
            var existingMember = _members.FirstOrDefault(m => m.MemberId == updatedMember.MemberId);

            if (existingMember != null)
            {
                existingMember.Name = updatedMember.Name;
                existingMember.Rank = updatedMember.Rank;
                return existingMember;
            }

            return null;
        }

        public Member? DeleteMember(int memberId)
        {
            var member = _members.FirstOrDefault(m => m.MemberId == memberId);
            if (member != null)
            {
                _members.Remove(member);
                return member;
            }

            return null;
        }

        public List<Member> GetMembers()
        {
            return new List<Member>(_members);
        }
    }
}