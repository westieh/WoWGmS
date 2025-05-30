using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

namespace WoWGMS.Pages.Shared
{
    public class GetAllMembersModel : PageModel
    {
        private readonly IMemberService _memberService;

        // Constructor injecting the member service
        public GetAllMembersModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // List to store officers
        public List<Member>? Officers { get; private set; }

        // List to store raiders
        public List<Member>? Raiders { get; private set; }

        // List to store trialists
        public List<Member>? Trialists { get; private set; }

        // Handles GET requests to the page
        public void OnGet()
        {
            // Fetch all members
            var members = _memberService.GetMembers();

            // Filter and assign members based on their rank
            Officers = members.Where(m => m.Rank == Rank.Officer).ToList();
            Raiders = members.Where(m => m.Rank == Rank.Raider).ToList();
            Trialists = members.Where(m => m.Rank == Rank.Trialist).ToList();
        }
    }
}
