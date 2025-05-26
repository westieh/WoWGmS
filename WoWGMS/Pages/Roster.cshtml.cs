using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;

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

        // For creating a new roster
        [BindProperty]
        public BossRoster NewRoster { get; set; }

        // Holds the roster currently being managed (viewed/edited)
        public BossRoster CreatedRoster { get; set; }

        // All rosters to list
        public List<BossRoster> AllRosters { get; set; } = new();

        // All available characters
        public List<Character> AllCharacters { get; set; } = new();

        // Used to bind the rosterId in Add/Remove participant forms
        [BindProperty]
        public int rosterId { get; set; }

        // Used to bind CharacterId when adding participant
        [BindProperty]
        public int CharacterId { get; set; }

        // Used to bind characterId when removing participant
        [BindProperty]
        public int characterId { get; set; }

        // Optional rosterId route parameter, so admin can load a specific roster to manage
        [BindProperty(SupportsGet = true)]
        public int? RosterId { get; set; }

        public void OnGet()
        {
            LoadPageData();
        }

        public IActionResult OnPostCreateRoster()
        {
            if (!ModelState.IsValid)
            {
                LoadPageData();
                return Page();
            }

            // Set creation date and instance time to now
            NewRoster.CreationDate = System.DateTime.Now;
            NewRoster.InstanceTime = System.DateTime.Now;

            // Add roster to DB
            var addedRoster = _rosterRepo.Add(NewRoster);

            // Reload all data with the newly created roster selected
            RosterId = addedRoster.RosterId;
            LoadPageData();

            // Clear the NewRoster form
            NewRoster = new BossRoster();

            return Page();
        }

        public IActionResult OnPostAddParticipant()
        {
            if (CharacterId == 0 || rosterId == 0)
            {
                LoadPageData();
                return Page();
            }

            var roster = _rosterRepo.GetById(rosterId);
            if (roster != null)
            {
                // Prevent duplicates
                if (!roster.Participants.Any(c => c.Id == CharacterId))
                {
                    var character = _characterService.GetCharacter(CharacterId);
                    if (character != null)
                    {
                        roster.Participants.Add(character);
                        _rosterRepo.Update(roster);
                    }
                }
            }

            RosterId = rosterId;
            LoadPageData();

            return Page();
        }

        public IActionResult OnPostRemoveParticipant()
        {
            if (rosterId == 0 || characterId == 0)
            {
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
            LoadPageData();

            return Page();
        }

        private void LoadPageData()
        {
            AllRosters = _rosterRepo.GetAll().ToList();
            AllCharacters = _characterService.GetAllCharacters();

            if (RosterId.HasValue)
            {
                CreatedRoster = _rosterRepo.GetById(RosterId.Value);
            }
            else
            {
                CreatedRoster = null;
            }
        }
    }
}
