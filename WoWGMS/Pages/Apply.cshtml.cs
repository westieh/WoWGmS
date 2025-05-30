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

        public ApplyModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [BindProperty]
        public Application Application { get; set; } = new Application();
        [BindProperty]
        public Dictionary<string, int> BossKills { get; set; } = new();

        public List<Boss> RaidBosses { get; set; } = new(); 
        public void OnGet()
        {
            RaidBosses = RaidRegistry.GetBossesForRaid("liberation-of-undermine");
        }

        public IActionResult OnPost()
        {
            RaidBosses = RaidRegistry.GetBossesForRaid("liberation-of-undermine");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                _applicationService.SubmitApplication(Application, BossKills);
                TempData["SuccessMessage"] = "Application submitted successfully!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong while submitting your application.");
                // Optionally log ex.Message
            }

            return Page();
        }

    }
}
