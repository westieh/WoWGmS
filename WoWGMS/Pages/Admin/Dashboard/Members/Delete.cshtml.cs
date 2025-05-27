using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Service;
using WowGMSBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WoWGMS.Pages.Admin.Dashboard.Members
{
    [Authorize(Roles = "Officer,Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IMemberService _memberService;

        public DeleteModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [BindProperty]
        public Member Member { get; set; }

        public IActionResult OnGet(int id)
        {
            Member = _memberService.GetMember(id);
            if (Member == null)
                return NotFound();

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                _memberService.DeleteMember(Member.MemberId);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("/Shared/GetAllMembers");
        }
    }
}
