using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;
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

        [BindProperty]
        public int rosterId { get; set; }

        [BindProperty]
        public int CharacterId { get; set; }

        [BindProperty]
        public int characterId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RosterId { get; set; }

        public BossRoster CreatedRoster { get; set; }

        public List<BossRoster> AllRosters { get; set; } = new();
        public List<Character> AllCharacters { get; set; } = new();
        public List<Raid> AllRaids { get; set; } = RaidRegistry.Raids;
        public List<Boss> BossesForSelectedRaid { get; set; } = new();

        public void OnGet()
        {
            LoadBossOptions();
            LoadPageData();
        }

        public IActionResult OnPostSelectRaid()
        {
            LoadBossOptions();
            LoadPageData();
            return Page();
        }

        public IActionResult OnPostCreateRoster()
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(SelectedRaidSlug) || string.IsNullOrEmpty(SelectedBossSlug))
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"[ModelState] {entry.Key}: {error.ErrorMessage}");
                    }
                }

                LoadBossOptions();
                LoadPageData();
                return Page();
            }

            var boss = RaidRegistry.GetBossesForRaid(SelectedRaidSlug)
                .FirstOrDefault(b => b.Slug == SelectedBossSlug);

            if (boss == null)
            {
                LoadBossOptions();
                LoadPageData();
                return Page();
            }

            NewRoster.RaidSlug = SelectedRaidSlug;
            NewRoster.BossSlug = boss.Slug;
            NewRoster.BossDisplayName = boss.DisplayName;
            NewRoster.CreationDate = System.DateTime.Now;
            NewRoster.InstanceTime = System.DateTime.Now;

            try
            {
                var addedRoster = _rosterRepo.Add(NewRoster);
                Console.WriteLine($"[EF] Roster created with ID {addedRoster.RosterId}");
                RosterId = addedRoster.RosterId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EF ERROR] {ex.Message}");
            }

            NewRoster = new BossRoster();
            LoadBossOptions();
            LoadPageData();
            return Page();
        }

        public IActionResult OnPostAddParticipant()
        {
            if (CharacterId == 0 || rosterId == 0)
            {
                LoadBossOptions();
                LoadPageData();
                return Page();
            }

            var roster = _rosterRepo.GetById(rosterId);
            if (roster != null && !roster.Participants.Any(c => c.Id == CharacterId))
            {
                var character = _characterService.GetCharacter(CharacterId);
                if (character != null)
                {
                    roster.Participants.Add(character);
                    _rosterRepo.Update(roster);
                }
            }

            RosterId = rosterId;
            LoadBossOptions();
            LoadPageData();
            return Page();
        }

        public IActionResult OnPostRemoveParticipant()
        {
            if (rosterId == 0 || characterId == 0)
            {
                LoadBossOptions();
                LoadPageData();
                return Page();
            }

            var roster = _rosterRepo.GetById(rosterId);
            if (roster != null)
            {
                var participant = roster.Participants.FirstOrDefault(c => c.Id == characterId);
                if (participant != null)
                {
                    roster.Participants.Remove(participant);
                    _rosterRepo.Update(roster);
                }
            }

            RosterId = rosterId;
            LoadBossOptions();
            LoadPageData();
            return Page();
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
