using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoWGMS.Repository;
using WoW.Model;

namespace WoWGMS.Pages.Admin.Dashboard.Applications
{
    public class EditModel : PageModel
    {
        private readonly ApplicationRepo _repo;

        public EditModel(ApplicationRepo repo)
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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _repo.UpdateApplication(Application);
            return RedirectToPage("/Admin/Dashboard/Index");
        }
    }
}