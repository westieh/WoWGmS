using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;

namespace WoWGMS.Pages
{
    public class EventModel : PageModel
    {
        private readonly IRosterRepository _rosterRepo;
        private readonly ICharacterService _characterService;

        public EventModel(IRosterRepository rosterRepo, ICharacterService characterService)
        {
            _rosterRepo = rosterRepo;
            _characterService = characterService;
        }

        public List<BossRoster> BossRosters { get; set; } = new();

        [BindProperty]
        public int SelectedRosterId { get; set; }

        [BindProperty]
        public int SelectedCharacterId { get; set; }

        public List<Character> MyCharacters { get; set; } = new();

        public void OnGet()
        {
            BossRosters = _rosterRepo.GetAll().ToList();

            // Get current user's member ID from claims or user manager
            var memberIdClaim = User.FindFirst("MemberId")?.Value;

            if (int.TryParse(memberIdClaim, out int memberId))
            {
                MyCharacters = _characterService.GetCharactersByMemberId(memberId);
            }
            else
            {
                // fallback or empty
                MyCharacters = new List<Character>();
            }
        }

            public IActionResult OnPostSignUp()
        {
            var roster = _rosterRepo.GetById(SelectedRosterId);
            var character = _characterService.GetCharacter(SelectedCharacterId);

            if (roster == null || character == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid roster or character selection.");
                // Reload data to redisplay form with error
                OnGet();
                return Page();
            }

            // Prevent duplicate sign-ups
            if (!roster.Participants.Any(p => p.Id == character.Id))
            {
                roster.Participants.Add(character);
                _rosterRepo.Update(roster);
            }

            return RedirectToPage();
        }
    }
}
