using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using static WoWGMS.Pages.MemberPanelModel;
using WowGMSBackend.Registry;
using WowGMSBackend.ViewModels;

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

            GroupedCharacters = _characterService.GetGroupedCharactersByBossSlug(SelectedBossSlug);
        }

        public IActionResult OnPostDeleteCharacter(int id)
        {
            _characterService.DeleteCharacter(id);
            return RedirectToPage(new { SelectedBossSlug });
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