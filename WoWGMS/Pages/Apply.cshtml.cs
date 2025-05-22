using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Service;
using WowGMSBackend.Model;

namespace WoWGMS.Pages
{
    public class ApplyModel : PageModel
    {
        private readonly IApplicationService _applicationService;

        public ApplyModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [BindProperty]
        public Application Application { get; set; } = new Application();

        public void OnGet()
        {
            // tom, vis form
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Application.SubmissionDate = DateTime.Now;
            Application.Approved = false;
            Application.Note = null;
            Application.ProcessedBy = null;

            _applicationService.AddApplication(Application);

            return RedirectToPage("ApplySuccess");
        }
    }
}