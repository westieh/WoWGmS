using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Registry;
using WowGMSBackend.Service;

namespace WoWGMS.Pages
{
    public class RosterModel : PageModel
    {
        private readonly IRosterService _rosterService;
        private readonly ICharacterService _characterService;

        // Constructor injecting roster and character services
        public RosterModel(IRosterService rosterService, ICharacterService characterService)
        {
            _rosterService = rosterService;
            _characterService = characterService;
        }

        // Binds new roster data
        [BindProperty]
        public BossRoster NewRoster { get; set; } = new();

        // Binds selected raid slug
        [BindProperty]
        public string SelectedRaidSlug { get; set; }

        // Binds selected boss slug
        [BindProperty]
        public string SelectedBossSlug { get; set; }

        // Binds roster ID for operations (supports GET)
        [BindProperty(SupportsGet = true)]
        public int? RosterId { get; set; }

        // Binds character ID for participant management
        [BindProperty]
        public int CharacterId { get; set; }

        // Binds participant ID for participant management
        [BindProperty]
        public int ParticipantId { get; set; }

        // Roster instance for detailed view
        public BossRoster CreatedRoster { get; set; }

        // List of all rosters
        public List<BossRoster> AllRosters { get; set; } = new();

        // List of all characters
        public List<Character> AllCharacters { get; set; } = new();

        // List of all raids from registry
        public List<Raid> AllRaids { get; set; } = RaidRegistry.Raids;

        // Bosses available for the selected raid
        public List<Boss> BossesForSelectedRaid { get; set; } = new();

        // Handles GET requests to initialize the page
        public void OnGet()
        {
            if (NewRoster.InstanceTime == default)
            {
                NewRoster.InstanceTime = DateTime.Now;
            }
            AllRosters = _rosterService.GetAllRosters().ToList();
            LoadBossOptions();
            LoadPageData();
        }

        // Handles POST requests to create a new roster
        public IActionResult OnPostCreateRoster()
        {
            try
            {
                _rosterService.CreateRoster(NewRoster, SelectedRaidSlug, SelectedBossSlug);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                LoadBossOptions();
                LoadPageData();
                return Page();
            }

            return RedirectToPage();
        }

        // Handles POST requests to select a raid and update boss options
        public IActionResult OnPostSelectRaid()
        {
            try
            {
                LoadBossOptions();
                LoadPageData();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to load raid data: {ex.Message}";
            }

            return Page();
        }

        // Handles POST requests to delete a roster
        public IActionResult OnPostDeleteRoster(int id)
        {
            try
            {
                var roster = _rosterService.GetRosterById(id);
                if (roster == null)
                {
                    TempData["Error"] = "Roster not found.";
                    return RedirectToPage();
                }

                _rosterService.Delete(roster.RosterId);
                TempData["Success"] = "Roster deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting roster: {ex.Message}";
            }

            return RedirectToPage();
        }

        // Loads page data: all rosters and characters, and selected roster if applicable
        private void LoadPageData()
        {
            AllRosters = _rosterService.GetAllRosters().ToList();
            AllCharacters = _characterService.GetCharacters();
            CreatedRoster = RosterId.HasValue ? _rosterService.GetRosterById(RosterId.Value) : null;
        }

        // Loads bosses for the selected raid
        private void LoadBossOptions()
        {
            BossesForSelectedRaid = string.IsNullOrEmpty(SelectedRaidSlug)
                ? new List<Boss>()
                : RaidRegistry.GetBossesForRaid(SelectedRaidSlug);
        }
    }
}
