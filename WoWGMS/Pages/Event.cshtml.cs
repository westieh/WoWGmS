using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WoWGMS.Pages
{
    public class EventModel : PageModel
    {
        private readonly IRosterService _rosterService;

        public EventModel(IRosterService rosterService)
        {
            _rosterService = rosterService;
        }

        public Dictionary<BossRoster, List<Character>> UpcomingRostersGrouped { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int RosterIndex { get; set; } = 0;

        public void OnGet()
        {
            var allRosters = _rosterService.GetUpcomingRosters();

            if (RosterIndex < 0 || RosterIndex >= allRosters.Count)
                RosterIndex = 0;

            var selectedRoster = allRosters.ElementAtOrDefault(RosterIndex);
            if (selectedRoster != null)
                UpcomingRostersGrouped = new Dictionary<BossRoster, List<Character>> { { selectedRoster, selectedRoster.Participants } };
            else
                UpcomingRostersGrouped = new Dictionary<BossRoster, List<Character>>();
        }
    }
}