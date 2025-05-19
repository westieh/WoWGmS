using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoWGMS.Repository;
using WoW.Model;

namespace WoWGMS.Pages.Admin.Dashboard.Members
{
    public class EditModel : PageModel
    {
        private readonly MemberRepo _repo;

        public EditModel(MemberRepo repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public Member Member { get; set; }

        public IActionResult OnGet(int id)
        {
            Member = _repo.GetMember(id);
            if (Member == null)
                return NotFound();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _repo.UpdateMember(Member.MemberId, Member);
            return RedirectToPage("/Admin/Dashboard/Index");
        }
    }
}