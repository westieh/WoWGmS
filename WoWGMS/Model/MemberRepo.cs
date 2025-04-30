using System.Collections.Generic;
using WoW.Model;

namespace WoW.Model
{
	public class MemberRepo
	{
		private List<Member> _members = new List<Member>();

		public Member AddMember(Member member)
		{
			_members.Add(member);
			return member;
		}

		public Member? GetMember(int memberId)
		{
			foreach (Member member in _members)
			{
				if (member.MemberId == memberId) return member;
			}
			return null;
		}

		//UpdateMember gør brug af lambda
		public Member? UpdateMember(int memberId, Member member)
		{
			var existingMember = _members.FirstOrDefault(m => m.MemberId == memberId);

			if (existingMember != null)
			{
				existingMember.MemberId = memberId;
				existingMember.Name = member.Name;
				existingMember.Rank = member.Rank;
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

		public Member? ChangeMemberRank(int actingMemberId, int targetMemberId, Rank newRank)
		{
			var actingMember = _members.FirstOrDefault(m => m.MemberId == actingMemberId);
			var targetMember = _members.FirstOrDefault(m => m.MemberId == targetMemberId);

			if (actingMember == null || targetMember == null)
				return null;

			if (actingMember.Rank != Rank.Officer)
				throw new UnauthorizedAccessException("Only officers can change ranks.");

			targetMember.Rank = newRank;
			return targetMember;
		}
	}
}

