using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

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

        [BindProperty]
        public List<BossKill> BossKills { get; set; } = new();
        public void OnGet()
        {
            Characters = _characterService.GetAllCharactersWithMemberAndBossKills();
        }
        public IActionResult OnPostUpdateBossKills()
        {
            if (BossKills == null || CharacterId == 0)
                return BadRequest();

            _bossKillService.SetBossKillsForCharacter(CharacterId, BossKills);

            TempData["SuccessMessage"] = "Boss kills updated.";
            return RedirectToPage();
        }
    }
}