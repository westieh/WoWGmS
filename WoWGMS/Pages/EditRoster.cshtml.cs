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

        // Constructor injecting the roster service
        public EditRosterModel(IRosterService rosterService)
        {
            _rosterService = rosterService;
        }

        // Binds the roster model to the page
        [BindProperty]
        public BossRoster Roster { get; set; }

        // List of eligible characters for the roster
        public List<Character> EligibleCharacters { get; set; } = new();

        // Handles GET requests, loads roster and eligible characters
        public IActionResult OnGet(int id)
        {
            Roster = _rosterService.GetRosterById(id);
            if (Roster == null)
                return NotFound();

            EligibleCharacters = _rosterService.GetEligibleCharacters(Roster);
            return Page();
        }

        // Handles POST request to update roster time
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

        // Handles adding a character to the roster
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
                // Silently ignore duplicate additions
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error adding character.");
            }

            return RedirectToPage(new { id });
        }

        // Handles removing a character from the roster
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
