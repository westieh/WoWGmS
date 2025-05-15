using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoW.Model;
using WoWGMS.Service;

namespace WoWGMS.Pages.Shared
{
    public class GetAllMembersModel : PageModel
    {
        private IMemberService _memberService;

        public GetAllMembersModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public List<Member>? Officers { get; private set; }
        public List<Member>? Raiders { get; private set; }
        public List<Member>? Trialists { get; private set; }

        public void OnGet()
        {
            // Hent alle medlemmer
            var members = _memberService.GetMembers();
            // Opdel medlemmerne baseret på deres rang
            Officers = members.Where(m => m.Rank == Rank.Officer).ToList();
            Raiders = members.Where(m => m.Rank == Rank.Raider).ToList();
            Trialists = members.Where(m => m.Rank == Rank.Trialist).ToList();
        }
    }
}
