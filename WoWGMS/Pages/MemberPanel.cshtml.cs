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

        public List<CharacterWithKill> CharactersForMember { get; set; } = new();

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
            Character ??= new Character();
            var memberId = LoggedInMemberId;
            if (memberId == null) return NotFound("MemberId claim missing.");

            AllBosses = (RaidRegistry.Raids ?? new List<Raid>())
                .Where(r => r?.Bosses != null)
                .SelectMany(r => r.Bosses)
                .Where(b => b != null && !string.IsNullOrEmpty(b.Slug) && !string.IsNullOrEmpty(b.DisplayName))
                .ToList();

            if (string.IsNullOrEmpty(SelectedBossSlug) &&
                AllBosses.Any(b => b.Slug == "vexie-and-the-geargrinders"))
            {
                SelectedBossSlug = "vexie-and-the-geargrinders";
            }

            CharactersForMember = _characterService
                .GetCharactersByMemberId(memberId.Value)
                .Select(c =>
                {
                    var kills = _bossKillService.GetBossKillsForCharacter(c.Id);

                    int killCount = !string.IsNullOrEmpty(SelectedBossSlug)
                        ? kills.Where(k => k.BossSlug == SelectedBossSlug).Sum(k => k.KillCount)
                        : kills.GroupBy(k => k.BossSlug).Select(g => g.Sum(x => x.KillCount)).DefaultIfEmpty(0).Max();

                    var top = kills
                        .GroupBy(k => k.BossSlug)
                        .Select(g => new { Count = g.Sum(x => x.KillCount), Example = g.First() })
                        .OrderByDescending(x => x.Count)
                        .FirstOrDefault();

                    return new CharacterWithKill
                    {
                        Character = c,
                        HighestKill = top?.Example,
                        KillCount = killCount
                    };
                }).ToList();

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

            Character.MemberId = memberId.Value;
            _characterService.AddCharacter(Character);

            var savedCharacter = _characterService
                .GetCharactersByMemberId(memberId.Value)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault(c =>
                    c.CharacterName == Character.CharacterName &&
                    c.RealmName == Character.RealmName &&
                    c.Class == Character.Class &&
                    c.Role == Character.Role);

            if (savedCharacter == null)
            {
                ModelState.AddModelError(string.Empty, "Character could not be retrieved after creation.");
                return Page();
            }

            var bossKills = BossKillInputs
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => new BossKill
                {
                    BossSlug = kvp.Key,
                    KillCount = kvp.Value
                }).ToList();

            _bossKillService.SetBossKillsForCharacter(savedCharacter.Id, bossKills);

            TempData["SuccessMessage"] = "Character created successfully!";
            return RedirectToPage();
        }


        public class CharacterWithKill
        {
            public Character Character { get; set; }
            public BossKill? HighestKill { get; set; }
            public int KillCount { get; set; }
        }
    }
}