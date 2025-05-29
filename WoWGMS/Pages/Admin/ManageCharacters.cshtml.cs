using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using static WoWGMS.Pages.MemberPanelModel;
using WowGMSBackend.Registry;

namespace WoWGMS.Pages.Admin
{
    [Authorize(Roles = "Officer")]
    public class ManageCharactersModel : PageModel
    {
        private readonly ICharacterService _characterService;
        private readonly IBossKillService _bossKillService;

        public ManageCharactersModel(ICharacterService characterService, IBossKillService bossKillService)
        {
            _characterService = characterService;
            _bossKillService = bossKillService;
        }

        public List<Character> Characters { get; set; } = new();
        [BindProperty]
        public int CharacterId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedBossSlug { get; set; }
        [BindProperty]
        public Dictionary<int, int> KillUpdates { get; set; } = new();

        public Dictionary<int, List<CharacterWithKill>> GroupedCharacters { get; set; } = new();
        public List<Boss> AllBosses { get; set; } = new();
        [BindProperty]
        public List<BossKill> BossKills { get; set; } = new();
        public void OnGet()
        {
            AllBosses = RaidRegistry.Raids
                .SelectMany(r => r.Bosses)
                .Where(b => b != null && b.Slug != null && b.DisplayName != null)
                .ToList();

            if (string.IsNullOrEmpty(SelectedBossSlug))
                SelectedBossSlug = "vexie-and-the-geargrinders";

            var allChars = _characterService.GetAllCharactersWithMemberAndBossKills();

            var withKills = allChars.Select(c =>
            {
                var kills = c.BossKills.Where(k => k.BossSlug == SelectedBossSlug).ToList();
                return new CharacterWithKill
                {
                    Character = c,

                    KillCount = kills.Sum(k => k.KillCount)
                };
            });

            GroupedCharacters = withKills
                .GroupBy(c => c.Character.MemberId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public IActionResult OnPostDeleteCharacter(int id)
        {
            _characterService.DeleteCharacter(id);
            return RedirectToPage(new { SelectedBossSlug });
        }

        public class CharacterWithKill
        {
            public Character Character { get; set; }
            public int KillCount { get; set; }
        }
        public IActionResult OnPostUpdateKills(string selectedBossSlug)
        {
            if (string.IsNullOrEmpty(selectedBossSlug))
                selectedBossSlug = "vexie-and-the-geargrinders";

            foreach (var entry in KillUpdates)
            {
                int characterId = entry.Key;
                int newCount = entry.Value;

                _bossKillService.SetBossKillsForCharacter(characterId, new List<BossKill>
        {
            new BossKill
            {
                BossSlug = selectedBossSlug,
                KillCount = newCount
            }
        });
            }

            return RedirectToPage(new { SelectedBossSlug = selectedBossSlug });
        }
    }
}