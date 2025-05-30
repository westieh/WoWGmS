using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Service;
using WowGMSBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WoWGMS.Pages.Admin.Dashboard.Members
{
    // Only users with the role Officer or Admin can access this page
    [Authorize(Roles = "Officer,Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IMemberService _memberService;

        // Constructor injects the MemberService dependency
        public DeleteModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // Member object bound to the form in the Razor page
        [BindProperty]
        public Member Member { get; set; }

        // Handles GET requests to retrieve the member for deletion confirmation
        public IActionResult OnGet(int id)
        {
            Member = _memberService.GetMember(id);

            if (Member == null)
                return NotFound(); // If no member is found, return 404

            return Page(); // Display the page with the member details
        }

        // Handles POST requests to actually delete the member
        public IActionResult OnPost()
        {
            try
            {
                _memberService.DeleteMember(Member.MemberId); // Perform the delete operation
            }
            catch (InvalidOperationException ex)
            {
                // If there's an error (e.g., trying to delete a non-deletable member), display error on the page
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            // Redirect back to the list of all members after successful deletion
            return RedirectToPage("/Shared/GetAllMembers");
        }
    }
}