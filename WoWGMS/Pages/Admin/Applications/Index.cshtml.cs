using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoWGMS.Repository;
using WoW.Model;
using Microsoft.AspNetCore.Authorization;

namespace WoWGMS.Pages.Admin.Applications
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationRepo _repo;

        public IndexModel(ApplicationRepo repo)
        {
            _repo = repo;
        }

        public List<WoW.Model.Application> Applications { get; set; }

        public void OnGet()
        {
            Applications = _repo.GetAllApplications();
        }
    }
}
