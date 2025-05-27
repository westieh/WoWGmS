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
            {
                return Page();
            }

            // Set default values (just to be safe)
            Application.Approved = false;
            Application.Note = null;
            Application.ProcessedBy = null;

            Application.BossKills = BossKills
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => new BossKill
                    {
                        BossSlug = kvp.Key,
                        KillCount = kvp.Value,
                        Application = Application
                    })
                    .ToList();
            // Submit through service (ensures ID and timestamp are set)
            _applicationService.SubmitApplication(Application);

            return Page();
        }
    }
}
