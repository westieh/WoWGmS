using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Registry;

namespace WoWGMS.Pages
{

    [Authorize(Roles = "Trialist,Raider,Officer")]
    public class MemberPanelModel : PageModel
    {
        private readonly ICharacterService _characterService;
        private readonly IMemberService _memberService;
        private readonly IBossKillService _bossKillService;

        public MemberPanelModel(ICharacterService characterService, IMemberService memberService, IBossKillService bossKillService)
        {
            _characterService = characterService;
            _memberService = memberService;
            _bossKillService = bossKillService;
        }
        [BindProperty(SupportsGet = true)]
        public string SelectedBossSlug { get; set; }
        [BindProperty]
        public Character Character { get; set; }

        [BindProperty]
        public Dictionary<string, int> BossKillInputs { get; set; } = new();

        public List<Boss> AllBosses { get; set; } = new();

        public List<Character> CharactersForMember { get; set; } = new();

        private int? LoggedInMemberId
        {
            get
            {
                var memberIdClaim = User.FindFirst("MemberId")?.Value;
                return int.TryParse(memberIdClaim, out var id) ? id : null;
            }
        }

        public IActionResult OnGet()
        {

            if (string.IsNullOrEmpty(SelectedBossSlug))
            {
                // Force navigation to the correct URL on initial page load
                return RedirectToPage(new { SelectedBossSlug = "vexie-and-the-geargrinders" });
            }
            var memberId = LoggedInMemberId;
            if (memberId == null)
                return NotFound("MemberId claim missing.");

            AllBosses = (RaidRegistry.Raids ?? new List<Raid>())
                .Where(r => r?.Bosses != null)
                .SelectMany(r => r.Bosses)
                .Where(b => !string.IsNullOrEmpty(b.Slug) && !string.IsNullOrEmpty(b.DisplayName))
                .ToList();

            if (string.IsNullOrEmpty(SelectedBossSlug) &&
                AllBosses.Any(b => b.Slug == "vexie-and-the-geargrinders"))
            {
                SelectedBossSlug = "vexie-and-the-geargrinders";
            }

            CharactersForMember = _characterService.GetCharactersByMemberId(memberId.Value);

            return Page();
        }

        public IActionResult OnPost()
        {
            AllBosses = (RaidRegistry.Raids ?? new List<Raid>())
                .Where(r => r?.Bosses != null)
                .SelectMany(r => r.Bosses)
                .Where(b => b != null && !string.IsNullOrEmpty(b.Slug) && !string.IsNullOrEmpty(b.DisplayName))
                .ToList();

            var memberId = LoggedInMemberId;
            if (memberId == null)
            {
                ModelState.AddModelError(string.Empty, "MemberId claim missing.");
                return Page();
            }

            if (!ModelState.IsValid) return Page();
            try
            {
                _characterService.CreateCharacterWithKills(Character, BossKillInputs, memberId.Value);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }


            TempData["SuccessMessage"] = "Character created successfully!";
            return RedirectToPage(new { SelectedBossSlug = this.SelectedBossSlug });
        }



    }
}