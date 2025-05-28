using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WowGMSBackend.Repository
{
    public class MemberRepo : IMemberRepo
    {
        private readonly WowDbContext _context;
        public MemberRepo(WowDbContext context)
        {
            _context = context;
        }


        public Member AddMember(Member member)
        {
            _context.Members.Add(member);
            _context.SaveChanges();
            return member;
        }

        public Member? GetMember(int memberId)
        {
            return _context.Members.FirstOrDefault(m => m.MemberId == memberId);
        }

        public Member? UpdateMember(int memberId, Member updatedMember)
        {
            var existingMember = _context.Members.FirstOrDefault(m => m.MemberId == memberId); 

            if (existingMember != null)
            {
                existingMember.Name = updatedMember.Name;
                existingMember.Rank = updatedMember.Rank;
                _context.SaveChanges();
                return existingMember;
            }

            return null;
        }

        public Member? DeleteMember(int memberId)
        {
            var member = _context.Members.FirstOrDefault(m => m.MemberId == memberId);
            if (member != null)
            {
                _context.Members.Remove(member);
                _context.SaveChanges();
                return member;
            }

            return null;
        }

        public List<Member> GetMembers()
        {
           return _context.Members
                .Include(m => m.Characters)
                .ToList();
        }
    }
}
