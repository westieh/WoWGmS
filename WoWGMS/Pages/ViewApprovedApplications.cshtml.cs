using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;

namespace WoWGMS.Pages
{
    [Authorize(Roles = "Officer")]
    public class ViewApprovedApplicationsModel : PageModel
    {
        private readonly IApplicationService _applicationService;

        public ViewApprovedApplicationsModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public List<Application> Applications { get; set; } = new();

        public void OnGet()
        {
            Applications = _applicationService
                .GetAllApplications()
                .Where(a => a.Approved)
                .ToList();
        }
    }
}
