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
    public class EditModel : PageModel
    {
        private readonly IMemberService _memberService;

        public EditModel(IMemberService memberService)
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
            if (!ModelState.IsValid)
                return Page();

            _memberService.UpdateMember(Member.MemberId, Member);
            return RedirectToPage("/Admin/Dashboard/Index");
        }
    }
}