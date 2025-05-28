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
        public EditMemberViewModel Member { get; set; }

        public IActionResult OnGet(int id)
        {
            var member = _memberService.GetMember(id);
            if (member == null) return NotFound();

            Member = new EditMemberViewModel
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Rank = member.Rank
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var existing = _memberService.GetMember(Member.MemberId);
            if (existing == null) return NotFound();

            existing.Name = Member.Name;
            existing.Rank = Member.Rank;

            _memberService.UpdateMember(existing.MemberId, existing);
            return RedirectToPage("/Shared/GetAllMembers");
        }
    }
}