using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Service;

public class ViewApplicationsModel : PageModel
{
    private readonly IApplicationService _applicationService;
    public List<Application> Applications { get; set; }

    public ViewApplicationsModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }
    public void OnGet()
    {

        Applications = _applicationService.GetAllApplications();
    }
    [BindProperty]
    public int ApplicationId { get; set; }

    [BindProperty]
    public string Note { get; set; }
    [BindProperty]
    public bool Approved { get; set; }


    public IActionResult OnPostUpdateNote()
    {
        var appToEdit = _applicationService.GetApplicationById(ApplicationId);

        if (appToEdit != null)
        {
            appToEdit.Note = Note;
            _applicationService.UpdateApplication(appToEdit);
        }

        return RedirectToPage(); // reloads the same page
    }
    public IActionResult OnPostToggleApproval()
    {
        var appToUpdate = _applicationService.GetApplicationById(ApplicationId);

        if (appToUpdate != null)
        {
            appToUpdate.Approved = Approved;
            _applicationService.UpdateApplication(appToUpdate);
        }

        return RedirectToPage();
    }



}
