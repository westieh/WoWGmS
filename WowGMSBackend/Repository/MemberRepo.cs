using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WowGMSBackend.Repository
{
    /// <summary>
    /// Repository for managing Member entities.
    /// </summary>
    public class MemberRepo : IMemberRepo
    {
        private readonly WowDbContext _context;

        public MemberRepo(WowDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new member to the database.
        /// </summary>
        public Member AddMember(Member member)
        {
            _context.Members.Add(member);
            _context.SaveChanges();
            return member;
        }

        /// <summary>
        /// Retrieves a member by ID.
        /// </summary>
        public Member? GetMember(int memberId)
        {
            return _context.Members.FirstOrDefault(m => m.MemberId == memberId);
        }

        /// <summary>
        /// Updates an existing member's name and rank.
        /// </summary>
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

        /// <summary>
        /// Deletes a member from the database.
        /// </summary>
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

        /// <summary>
        /// Retrieves all members including their related characters.
        /// </summary>
        public List<Member> GetMembers()
        {
            return _context.Members
                .Include(m => m.Characters)
                .ToList();
        }
    }
}
