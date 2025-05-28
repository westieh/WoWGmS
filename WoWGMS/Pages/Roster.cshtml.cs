using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Registry;

namespace WoWGMS.Pages
{
    public class RosterModel : PageModel
    {
        private readonly IRosterRepository _rosterRepo;
        private readonly ICharacterService _characterService;

        public RosterModel(IRosterRepository rosterRepo, ICharacterService characterService)
        {
            _rosterRepo = rosterRepo;
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
            AllRosters = _rosterRepo.GetAll().ToList();
            LoadBossOptions();
            LoadPageData();
        }

        public IActionResult OnPostCreateRoster()
        {
            if (string.IsNullOrEmpty(SelectedRaidSlug) || string.IsNullOrEmpty(SelectedBossSlug))
            {
                ModelState.AddModelError(string.Empty, "Missing raid or boss selection.");
                LoadBossOptions();
                LoadPageData();
                return Page();
            }

            var boss = RaidRegistry.GetBossesForRaid(SelectedRaidSlug)
                                   .FirstOrDefault(b => b.Slug == SelectedBossSlug);

            if (boss == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid boss selected.");
                LoadBossOptions();
                LoadPageData();
                return Page();
            }

            NewRoster.RaidSlug = SelectedRaidSlug;
            NewRoster.BossSlug = boss.Slug;
            NewRoster.BossDisplayName = boss.DisplayName;
            NewRoster.CreationDate = DateTime.Now;

            if (NewRoster.InstanceTime == default)
                NewRoster.InstanceTime = DateTime.Now.AddHours(1);

            _rosterRepo.Add(NewRoster);

            return RedirectToPage();
        }

        public IActionResult OnPostSelectRaid()
        {
            LoadBossOptions();
            LoadPageData();
            return Page();
        }

        public IActionResult OnPostDeleteRoster(int id)
        {
            var roster = _rosterRepo.GetById(id);
            if (roster != null)
            {
                _rosterRepo.Delete(roster.RosterId);
            }
            return RedirectToPage();
        }

        private void LoadPageData()
        {
            AllRosters = _rosterRepo.GetAll().ToList();
            AllCharacters = _characterService.GetCharacters();
            CreatedRoster = RosterId.HasValue ? _rosterRepo.GetById(RosterId.Value) : null;
        }

        private void LoadBossOptions()
        {
            BossesForSelectedRaid = string.IsNullOrEmpty(SelectedRaidSlug)
                ? new List<Boss>()
                : RaidRegistry.GetBossesForRaid(SelectedRaidSlug);
        }
    }
}
