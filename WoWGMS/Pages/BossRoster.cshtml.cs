using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoW.Model;
using WoWGMS.Service;

namespace WoWGMS.Pages
{
    public class BossRosterModel : PageModel
    {
        private readonly IMemberService _memberService;

        public BossRosterModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public List<Member> Officers { get; private set; } = new();
        public List<Member> Raiders { get; private set; } = new();
        public List<Member> Trialists { get; private set; } = new();

        public static List<Member> Roster { get; set; } = new();

        [BindProperty]
        public int SelectedMemberId { get; set; }

        public void OnGet()
        {
            LoadMembers();
        }

        public IActionResult OnPost()
        {
            LoadMembers();

            var allMembers = Officers.Concat(Raiders).Concat(Trialists).ToList();
            var selected = allMembers.FirstOrDefault(m => m.MemberId == SelectedMemberId);

            if (selected != null && !Roster.Any(m => m.MemberId == selected.MemberId))
            {
                Roster.Add(selected);
            }

            return Page();
        }

        private void LoadMembers()
        {
            var members = _memberService.GetMembers();
            Officers = members.Where(m => m.Rank == Rank.Officer).ToList();
            Raiders = members.Where(m => m.Rank == Rank.Raider).ToList();
            Trialists = members.Where(m => m.Rank == Rank.Trialist).ToList();
        }
    }
}
