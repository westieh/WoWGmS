using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;

namespace WoWGMS.Pages
{
    public class EditRosterModel : PageModel
    {

        private readonly IRosterService _rosterService;

        public EditRosterModel( IRosterService rosterService)
        {

            _rosterService = rosterService;
        }

        [BindProperty]
        public BossRoster Roster { get; set; }

        public List<Character> EligibleCharacters { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            Roster = _rosterService.GetRosterById(id);
            if (Roster == null) return NotFound();

            EligibleCharacters = _rosterService.GetEligibleCharacters(Roster);
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            try
            {
                _rosterService.UpdateRosterTime(id, Roster.InstanceTime);
                TempData["SuccessMessage"] = "Roster updated successfully.";
                return RedirectToPage("/Roster");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to update the roster.");
                return Page();
            }
        }

        public IActionResult OnPostAdd(int id, int characterId)
        {
            try
            {
                var character = _rosterService.GetCharacterById(characterId);
                if (character == null)
                {
                    ModelState.AddModelError(string.Empty, "Character not found.");
                    return RedirectToPage(new { id });
                }

                _rosterService.AddCharacterToRoster(id, character);
                TempData["SuccessMessage"] = "Character added.";
            }
            catch (InvalidOperationException)
            {
                // No need to show error; silently ignore duplicate
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error adding character.");
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostRemove(int id, int participantId)
        {
            try
            {
                _rosterService.RemoveCharacterFromRoster(id, participantId);
                TempData["SuccessMessage"] = "Participant removed.";
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove participant.");
            }

            return RedirectToPage(new { id });
        }

    }
}
