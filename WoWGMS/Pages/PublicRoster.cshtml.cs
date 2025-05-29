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

        public PublicRosterModel(IRosterRepository rosterRepo, IBossKillService bossKillService)
        {
            _rosterRepo = rosterRepo;
            _bossKillService = bossKillService;
        }

        public BossRoster Roster { get; set; }

        public Dictionary<int, int> BossKillCounts { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            Roster = _rosterRepo.GetById(id);
            if (Roster == null) return NotFound();

            BossKillCounts = _bossKillService.GetBossKillCountsForRoster(Roster);

            return Page();
        }
    }
}
