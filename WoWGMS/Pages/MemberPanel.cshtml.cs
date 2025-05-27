using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;


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

        [BindProperty]
        public Character Character { get; set; }

        public List<CharacterWithKill> CharactersForMember { get; set; } = new();


        // Helper property to get the logged-in member's ID from claims
        private int? LoggedInMemberId
        {
            get
            {
                var memberIdClaim = User.FindFirst("MemberId")?.Value;
                if (int.TryParse(memberIdClaim, out var memberId))
                    return memberId;
                return null;
            }
        }

        public IActionResult OnGet()
        {
            var memberId = LoggedInMemberId;
            if (memberId == null)
                return NotFound("MemberId claim missing.");

            var member = _memberService.GetMember(memberId.Value);
            if (member == null)
                return NotFound("Member not found.");

            CharactersForMember = _characterService
                .GetCharactersByMemberId(memberId.Value)
                .Select(c => new CharacterWithKill
                    {
                        Character = c,
                        HighestKill = _bossKillService.GetMostKilledBossForCharacter(c.Id)
                    })
    .               ToList();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Model error on '{kvp.Key}': {error.ErrorMessage}");
                    }
                }

                Console.WriteLine("Exiting early due to invalid model state");
                return Page();
            }
            var memberId = LoggedInMemberId;
            if (memberId == null)
            {
                ModelState.AddModelError(string.Empty, "MemberId claim missing.");
                return Page();
            }

            var member = _memberService.GetMember(memberId.Value);
            if (member == null)
            {
                ModelState.AddModelError(string.Empty, "Member not found.");
                return Page();
            }
            

            Character.MemberId = memberId.Value;
           

            _characterService.AddCharacter(Character);

            TempData["SuccessMessage"] = "Character created successfully!";

            return RedirectToPage();
        }
        public class CharacterWithKill
        {
            public Character Character { get; set; }
            public BossKill? HighestKill { get; set; }
        }

    }

}
