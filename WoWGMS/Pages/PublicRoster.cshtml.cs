using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

namespace WoWGMS.Pages
{
    public class PublicRosterModel : PageModel
    {
        private readonly IRosterRepository _rosterRepo;
        private readonly IBossKillService _bossKillService;

        // Constructor injecting the roster repository and boss kill service
        public PublicRosterModel(IRosterRepository rosterRepo, IBossKillService bossKillService)
        {
            _rosterRepo = rosterRepo;
            _bossKillService = bossKillService;
        }

        // The roster being displayed
        public BossRoster Roster { get; set; }

        // Mapping of character ID to their boss kill counts
        public Dictionary<int, int> BossKillCounts { get; set; } = new();

        // Handles GET request to load the roster by ID
        public IActionResult OnGet(int id)
        {
            try
            {
                Roster = _rosterRepo.GetById(id);
                if (Roster == null)
                {
                    TempData["Error"] = "Roster not found.";
                    return RedirectToPage("/Error");
                }

                // Retrieve boss kill counts for roster participants
                BossKillCounts = _bossKillService.GetBossKillCountsForRoster(Roster);
                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the roster.";
                return RedirectToPage("/Error");
            }
        }
    }
}
