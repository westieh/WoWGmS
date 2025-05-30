using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Registry;

namespace WoWGMS.Pages
{
    public class ApplyModel : PageModel
    {
        private readonly IApplicationService _applicationService;

        // Constructor injecting the application service
        public ApplyModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        // Binds the application form data
        [BindProperty]
        public Application Application { get; set; } = new Application();

        // Binds the boss kills data
        [BindProperty]
        public Dictionary<string, int> BossKills { get; set; } = new();

        // List of bosses for the raid
        public List<Boss> RaidBosses { get; set; } = new();

        // Handles GET requests to initialize raid bosses
        public void OnGet()
        {
            RaidBosses = RaidRegistry.GetBossesForRaid("liberation-of-undermine");
        }

        // Handles POST requests to submit an application
        public IActionResult OnPost()
        {
            // Reinitialize raid bosses in case of re-render
            RaidBosses = RaidRegistry.GetBossesForRaid("liberation-of-undermine");

            // Validate the model
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Submit the application with boss kill data
                _applicationService.SubmitApplication(Application, BossKills);

                // Set success message in TempData
                TempData["SuccessMessage"] = "Application submitted successfully!";
            }
            catch (Exception ex)
            {
                // Set error message if submission fails
                ModelState.AddModelError(string.Empty, "Something went wrong while submitting your application.");
                // Optionally log ex.Message
            }

            return Page();
        }
    }
}
