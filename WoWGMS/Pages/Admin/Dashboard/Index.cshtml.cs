using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

namespace WoWGMS.Pages.Admin.Dashboard
{
    // Restricts access to users with the Admin role
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationRepo _applicationRepo;
        private readonly MemberRepo _memberRepo;

        // Constructor that injects repositories for applications and members
        public IndexModel(ApplicationRepo applicationRepo, MemberRepo memberRepo)
        {
            _applicationRepo = applicationRepo;
            _memberRepo = memberRepo;
        }

        // List to store retrieved applications
        public List<Application> Applications { get; set; }

        // List to store retrieved members
        public List<Member> Members { get; set; }

        // Handles GET requests to the page
        public void OnGet()
        {
            // Fetch all applications from the repository
            Applications = _applicationRepo.GetApplications();

            // Fetch all members from the repository
            Members = _memberRepo.GetMembers();
        }
    }
}
