using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

namespace WoWGMS.Pages
{
    public class EventModel : PageModel
    {
        private readonly IRosterService _rosterService;

        // Constructor injecting the roster service
        public EventModel(IRosterService rosterService)
        {
            _rosterService = rosterService;
        }

        // Holds the mapping of rosters to their participants
        public Dictionary<BossRoster, List<Character>> UpcomingRostersGrouped { get; set; } = new();

        // Bound property to track the index of the selected roster
        [BindProperty(SupportsGet = true)]
        public int RosterIndex { get; set; } = 0;

        // Handles GET requests to load the roster data
        public IActionResult OnGet()
        {
            try
            {
                // Retrieve all upcoming rosters
                var allRosters = _rosterService.GetUpcomingRosters();

                // Ensure the index is within valid bounds
                if (RosterIndex < 0 || RosterIndex >= allRosters.Count)
                    RosterIndex = 0;

                // Select the roster based on the current index
                var selectedRoster = allRosters.ElementAtOrDefault(RosterIndex);

                if (selectedRoster != null)
                {
                    // Map the selected roster to its participants
                    UpcomingRostersGrouped = new Dictionary<BossRoster, List<Character>>
                    {
                        { selectedRoster, selectedRoster.Participants }
                    };
                }
                else
                {
                    // No rosters found, initialize to empty and set info message
                    UpcomingRostersGrouped = new();
                    TempData["InfoMessage"] = "No upcoming rosters found.";
                }

                return Page();
            }
            catch (Exception)
            {
                // Handle unexpected errors during data load
                ModelState.AddModelError(string.Empty, "Failed to load roster data.");
                UpcomingRostersGrouped = new();
                return Page();
            }
        }
    }
}
