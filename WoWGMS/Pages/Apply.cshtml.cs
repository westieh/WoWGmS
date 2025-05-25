using System;
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
            // Display empty form on GET
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set default values (just to be safe)
            Application.Approved = false;
            Application.Note = null;
            Application.ProcessedBy = null;

            // Submit through service (ensures ID and timestamp are set)
            _applicationService.SubmitApplication(Application);

            return Page();
        }
    }
}
