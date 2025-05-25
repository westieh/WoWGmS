using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;

namespace WoWGMS.Pages.Admin.Dashboard.Members
{
    public class DeleteModel : PageModel
    {
        private readonly MemberRepo _repo;

        public DeleteModel(MemberRepo repo)
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
            try
            {
                _repo.DeleteMember(Member.MemberId);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("../Index");
        }
    }
}
