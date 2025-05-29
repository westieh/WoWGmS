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

        public RosterModel(IRosterService rosterService, ICharacterService characterService)
        {
            _rosterService = rosterService;
            _characterService = characterService;
        }

        [BindProperty]
        public BossRoster NewRoster { get; set; } = new();

        [BindProperty]
        public string SelectedRaidSlug { get; set; }

        [BindProperty]
        public string SelectedBossSlug { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RosterId { get; set; }

        [BindProperty]
        public int CharacterId { get; set; }

        [BindProperty]
        public int ParticipantId { get; set; }

        public BossRoster CreatedRoster { get; set; }
        public List<BossRoster> AllRosters { get; set; } = new();
        public List<Character> AllCharacters { get; set; } = new();
        public List<Raid> AllRaids { get; set; } = RaidRegistry.Raids;
        public List<Boss> BossesForSelectedRaid { get; set; } = new();

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

        private void LoadPageData()
        {
            AllRosters = _rosterService.GetAllRosters().ToList();
            AllCharacters = _characterService.GetCharacters();
            CreatedRoster = RosterId.HasValue ? _rosterService.GetRosterById(RosterId.Value) : null;
        }

        private void LoadBossOptions()
        {
            BossesForSelectedRaid = string.IsNullOrEmpty(SelectedRaidSlug)
                ? new List<Boss>()
                : RaidRegistry.GetBossesForRaid(SelectedRaidSlug);
        }
    }
}
