using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoW.Model;

namespace WoWGMS.Pages
{
    public class EventModel : PageModel
    {
        public List<Member> Roster { get; set; }

        public void OnGet()
        {
            Roster = BossRosterModel.Roster;
        }
    }
}