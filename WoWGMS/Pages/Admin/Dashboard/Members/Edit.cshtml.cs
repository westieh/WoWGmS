using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using WowGMSBackend.Model;

namespace WoWGMS.Pages.Admin.Dashboard.Members
{
    // Only users with the role Officer or Admin can access this page
    [Authorize(Roles = "Officer,Admin")]
    public class EditModel : PageModel
    {
        private readonly IMemberService _memberService;

        // Constructor injecting the MemberService dependency
        public EditModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // The view model that will be used for form binding in the Razor Page
        [BindProperty]
        public EditMemberViewModel Member { get; set; }

        // Handles GET requests to pre-fill the form with existing member data
        public IActionResult OnGet(int id)
        {
            var member = _memberService.GetMember(id);
            if (member == null) return NotFound(); // If no member is found, return 404

            // Map entity to ViewModel
            Member = new EditMemberViewModel
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Rank = member.Rank
            };

            return Page(); // Return page with populated model
        }

        // Handles POST requests to update the member
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page(); // Validate the form inputs

            var existing = _memberService.GetMember(Member.MemberId);
            if (existing == null) return NotFound(); // If the member doesn't exist, return 404

            // Update fields
            existing.Name = Member.Name;
            existing.Rank = Member.Rank;

            // Persist the updated member
            _memberService.UpdateMember(existing.MemberId, existing);

            // Redirect to the list of all members after successful update
            return RedirectToPage("/Shared/GetAllMembers");
        }
    }
}
