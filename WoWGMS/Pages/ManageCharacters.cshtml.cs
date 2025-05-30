using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

namespace WoWGMS.Pages
{
    public class ManageCharactersModel : PageModel
    {
        private readonly ICharacterService _characterService;
        private readonly IBossKillService _bossKillService;

        // Constructor injecting character and boss kill services
        public ManageCharactersModel(ICharacterService characterService, IBossKillService bossKillService)
        {
            _characterService = characterService;
            _bossKillService = bossKillService;
        }

        // List of characters with their boss kills
        public List<Character> Characters { get; set; } = new();

        // List of all bosses available
        public List<Boss> AllBosses { get; set; } = new();

        // Selected boss slug for displaying and updating kills
        [BindProperty(SupportsGet = true)]
        public string SelectedBossSlug { get; set; } = "vexie-and-the-geargrinders";

        // Handles GET requests to load characters and bosses
        public void OnGet()
        {
            Characters = _characterService.GetCharacters(); // includes BossKills
            AllBosses = _bossKillService.GetAllBosses();
        }

        // Handles POST requests to update a character's kill count
        public IActionResult OnPostUpdateKill(int characterId, int killCount, string selectedBossSlug)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedBossSlug))
                    selectedBossSlug = "vexie-and-the-geargrinders";

                _bossKillService.SetOrUpdateSingleBossKill(characterId, selectedBossSlug, killCount);

                TempData["SuccessMessage"] = "Kill count updated successfully.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to update kill count: {ex.Message}");
            }

            return RedirectToPage(new { SelectedBossSlug = selectedBossSlug });
        }

        // Handles POST requests to delete a character
        public IActionResult OnPostDeleteCharacter(int id)
        {
            try
            {
                _characterService.DeleteCharacter(id);
                TempData["SuccessMessage"] = "Character deleted successfully.";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to delete character: {ex.Message}");
            }

            return RedirectToPage(new { SelectedBossSlug });
        }
    }
}
