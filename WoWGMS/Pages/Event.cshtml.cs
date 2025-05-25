using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Model;

using WowGMSBackend.Repository;

namespace WoWGMS.Pages
{
    public class EventModel : PageModel
    {
        private readonly IRosterRepository _rosterRepo;

        public EventModel(IRosterRepository rosterRepo)
        {
            _rosterRepo = rosterRepo;
        }

        public List<BossRoster> BossRosters { get; set; } = new();

        public void OnGet()
        {
            BossRosters = _rosterRepo.GetAll().ToList();
        }
    }
}
