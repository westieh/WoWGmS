using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoWGMS.Repository;

namespace WoWGMS.Pages.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationRepo _applicationRepo;
        private readonly MemberRepo _memberRepo;

        public IndexModel(ApplicationRepo applicationRepo, MemberRepo memberRepo)
        {
            _applicationRepo = applicationRepo;
            _memberRepo = memberRepo;
        }

        public List<WoW.Model.Application> Applications { get; set; }
        public List<WoW.Model.Member> Members { get; set; }

        public void OnGet()
        {
            Applications = _applicationRepo.GetApplications();
            Members = _memberRepo.GetMembers();
        }
    }
}
