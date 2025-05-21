using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Collections.Generic;
using WoW.Model;
using WoWGMS.Repository;
using WowGMSBackend.MockData;
using WowGMSBackend.Service;

[Authorize(Roles = "Admin")]
public class ViewApplicationsModel : PageModel
{
    private readonly IApplicationService _applicationService;

    public ViewApplicationsModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [BindProperty]
    public List<Application> Applications { get; set; }

    [BindProperty]
    public int ApplicationId { get; set; }

    [BindProperty]
    public string Note { get; set; }

    [BindProperty]
    public bool Approved { get; set; }

    public void OnGet()
    {
        Applications = _applicationService.GetAllApplications();
    }

    public IActionResult OnPostUpdateNote()
    {
        var application = _applicationService.GetAllApplications()
            .FirstOrDefault(a => a.ApplicationId == ApplicationId);

        if (application != null)
        {
            application.Note = Note;
        }

        Applications = _applicationService.GetAllApplications();
        return Page();
    }

    public IActionResult OnPostToggleApproval()
    {
        var application = _applicationService.GetAllApplications()
            .FirstOrDefault(a => a.ApplicationId == ApplicationId);

        if (application != null)
        {
            if (Approved && !application.Approved)
            {
                _applicationService.ApproveApplication(application);
            }
            else if (!Approved && application.Approved)
            {
                application.Approved = false;
                // Optional: remove the member again if needed
            }
        }

        Applications = _applicationService.GetAllApplications();
        return Page();
    }
}
