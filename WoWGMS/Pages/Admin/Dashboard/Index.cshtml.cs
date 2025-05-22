using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoWGMS.Repository;
using WowGMSBackend.Model;

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

        public List<Application> Applications { get; set; }
        public List<WoW.Model.Member> Members { get; set; }

        public void OnGet()
        {
            Applications = _applicationRepo.GetAllApplications();
            Members = _memberRepo.GetMembers();
        }
    }
}
