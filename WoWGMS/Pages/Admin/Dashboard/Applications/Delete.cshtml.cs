using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoWGMS.Repository;
using WoW.Model;

namespace WoWGMS.Pages.Admin.Dashboard.Applications
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationRepo _repo;

        public DeleteModel(ApplicationRepo repo)
        {
            _repo = repo;
        }

        [BindProperty]
        public Application Application { get; set; }

        public IActionResult OnGet(int id)
        {
            Application = _repo.GetApplication(id);
            if (Application == null)
                return NotFound();
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            _repo.RemoveApplication(id);
            return RedirectToPage("/Admin/Dashboard/Index");
        }
    }
}