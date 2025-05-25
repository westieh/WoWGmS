using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
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
        public List<Member> Members { get; set; }

        public void OnGet()
        {
            Applications = _applicationRepo.GetApplications();
            Members = _memberRepo.GetMembers();
        }
    }
}
