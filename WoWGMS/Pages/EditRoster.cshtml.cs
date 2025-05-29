using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;

namespace WoWGMS.Pages
{
    public class EditRosterModel : PageModel
    {
        private readonly IRosterRepository _rosterRepo;
        private readonly ICharacterService _characterService;
        private readonly IBossKillService _bossKillService;
        private readonly IRosterService _rosterService;

        public EditRosterModel(IRosterRepository rosterRepo, ICharacterService characterService, IBossKillService bossKillService, IRosterService rosterService)
        {
            _rosterRepo = rosterRepo;
            _characterService = characterService;
            _bossKillService = bossKillService;
            _rosterService = rosterService;
        }

        [BindProperty]
        public BossRoster Roster { get; set; }

        public List<Character> EligibleCharacters { get; set; } = new();
        public Dictionary<int, int> BossKillCounts { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            Roster = _rosterRepo.GetById(id);
            if (Roster == null) return NotFound();

            LoadEligibleCharactersAndKills();
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var original = _rosterRepo.GetById(id);
            if (original == null) return NotFound();

            original.InstanceTime = Roster.InstanceTime;
            _rosterRepo.Update(original);
            return RedirectToPage("/Roster");
        }

        public IActionResult OnPostAdd(int id, int characterId)
        {
            if (_rosterRepo.GetById(id)?.Participants.Count >= 20)
                return RedirectToPage(new { id });

            var character = _characterService.GetCharacter(characterId);
            if (character == null)
                return RedirectToPage(new { id });

            try
            {
                _rosterService.AddCharacterToRoster(id, character);
            }
            catch (InvalidOperationException)
            {
                // Character already in roster — ignore
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostRemove(int id, int participantId)
        {
            var roster = _rosterRepo.GetById(id);
            if (roster == null) return RedirectToPage(new { id });

            var participant = roster.Participants.FirstOrDefault(p => p.Id == participantId);
            if (participant != null)
            {
                roster.Participants.Remove(participant);
                _rosterRepo.Update(roster);
            }

            return RedirectToPage(new { id });
        }

        private void LoadEligibleCharactersAndKills()
        {
            var existingIds = Roster.Participants.Select(p => p.Id).ToHashSet();
            EligibleCharacters = _characterService.GetCharacters()
                .Where(c => !existingIds.Contains(c.Id))
                .ToList();

            foreach (var c in EligibleCharacters)
            {
                var kills = _bossKillService.GetBossKillsForCharacter(c.Id);
                BossKillCounts[c.Id] = kills
                    .Where(k => k.BossSlug == Roster.BossSlug)
                    .Sum(k => k.KillCount);
            }
        }
    }
}
