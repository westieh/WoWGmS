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

        // Constructor injecting the application service
        public ViewApprovedApplicationsModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        // List of approved applications
        public List<Application> Applications { get; set; } = new();

        // Handles GET request to load all approved applications
        public void OnGet()
        {
            try
            {
                Applications = _applicationService
                    .GetAllApplications()
                    .Where(a => a.Approved) // Filter only approved applications
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to load approved applications: {ex.Message}";
                Applications = new List<Application>();
            }
        }
    }
}
