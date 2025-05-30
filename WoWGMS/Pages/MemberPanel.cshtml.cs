using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

namespace WoWGMS.Pages
{
    public class MemberPanelModel : PageModel
    {
        private readonly ICharacterService _characterService;
        private readonly IMemberService _memberService;
        private readonly IBossKillService _bossKillService;

        // Constructor injecting character, member, and boss kill services
        public MemberPanelModel(ICharacterService characterService, IMemberService memberService, IBossKillService bossKillService)
        {
            _characterService = characterService;
            _memberService = memberService;
            _bossKillService = bossKillService;
        }

        // Bound property for selected boss slug (supports GET)
        [BindProperty(SupportsGet = true)]
        public string SelectedBossSlug { get; set; }

        // Bound property for character input
        [BindProperty]
        public Character Character { get; set; }

        // Bound property for boss kill input mappings
        [BindProperty]
        public Dictionary<string, int> BossKillInputs { get; set; } = new();

        // List of all bosses
        public List<Boss> AllBosses { get; set; } = new();

        // List of characters belonging to the logged-in member
        public List<Character> CharactersForMember { get; set; } = new();

        // Handles GET requests to load member characters and boss list
        public IActionResult OnGet()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedBossSlug))
                    return RedirectToPage(new { SelectedBossSlug = "vexie-and-the-geargrinders" });

                var memberId = _memberService.GetLoggedInMemberId(User);
                if (memberId == null)
                    return NotFound("MemberId claim missing.");

                CharactersForMember = _characterService.GetCharactersByMemberId(memberId.Value);
                AllBosses = _bossKillService.GetAllBosses();

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while loading your characters.");
                return Page();
            }
        }

        // Handles POST requests to create a new character with associated boss kills
        public IActionResult OnPost()
        {
            AllBosses = _bossKillService.GetAllBosses();

            var memberId = _memberService.GetLoggedInMemberId(User);
            if (memberId == null)
            {
                ModelState.AddModelError(string.Empty, "MemberId claim missing.");
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            try
            {
                _characterService.CreateCharacterWithKills(Character, BossKillInputs, memberId.Value);
                TempData["SuccessMessage"] = "Character created successfully!";
                return RedirectToPage(new { SelectedBossSlug });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Failed to create character.");
                return Page();
            }
        }
    }
}
