using System.Security.Claims;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

namespace WowGMSBackend.Service
{
    /// <summary>
    /// Service class responsible for member-related operations such as login validation, 
    /// member creation, retrieval, updates, and deletion.
    /// </summary>
    public class MemberService : IMemberService
    {
        private readonly IMemberRepo _memberRepo;

        /// <summary>
        /// Initializes the service with the required repository dependency.
        /// </summary>
        public MemberService(IMemberRepo memberRepo)
        {
            _memberRepo = memberRepo;
        }

        /// <summary>
        /// Extracts and returns the logged-in member's ID from the given ClaimsPrincipal.
        /// </summary>
        public int? GetLoggedInMemberId(ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst("MemberId")?.Value;
            return int.TryParse(idClaim, out var id) ? id : null;
        }

        /// <summary>
        /// Validates login credentials against stored members. Returns the member if successful, otherwise null.
        /// </summary>
        public Member? ValidateLogin(string? name, string? password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                return null;

            var members = _memberRepo.GetMembers();
            return members.FirstOrDefault(m =>
                m.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                m.Password == password);
        }

        /// <summary>
        /// Adds a new member to the system after basic validation.
        /// Throws ArgumentException if name is empty.
        /// </summary>
        public Member AddMember(Member member)
        {
            if (string.IsNullOrWhiteSpace(member.Name))
                throw new ArgumentException("Name cannot be empty");

            return _memberRepo.AddMember(member);
        }

        /// <summary>
        /// Retrieves a member by their unique identifier.
        /// </summary>
        public Member? GetMember(int memberId)
        {
            return _memberRepo.GetMember(memberId);
        }

        /// <summary>
        /// Retrieves all members in the system.
        /// </summary>
        public List<Member> GetMembers()
        {
            return _memberRepo.GetMembers();
        }

        /// <summary>
        /// Updates member details based on their ID.
        /// </summary>
        public Member? UpdateMember(int memberId, Member updatedMember)
        {
            return _memberRepo.UpdateMember(memberId, updatedMember);
        }

        /// <summary>
        /// Deletes a member if they exist and are not an officer.
        /// Throws InvalidOperationException if attempting to delete an officer.
        /// </summary>
        public Member? DeleteMember(int memberId)
        {
            var member = _memberRepo.GetMember(memberId);
            if (member == null) return null;

            if (member.Rank == Rank.Officer)
                throw new InvalidOperationException("Cannot delete an officer");

            return _memberRepo.DeleteMember(memberId);
        }

        /// <summary>
        /// Changes the rank of a target member if the acting member has Officer rank.
        /// Throws UnauthorizedAccessException if acting member is not an officer.
        /// Returns null if either member is not found.
        /// </summary>
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

        /// <summary>
        /// Retrieves a member by their unique name, case-insensitive match.
        /// </summary>
        public Member? GetMemberByName(string name)
        {
            return _memberRepo.GetMembers()
                .FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
