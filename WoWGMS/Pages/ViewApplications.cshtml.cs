using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Collections.Generic;
using WoW.Model;

public class ViewApplicationsModel : PageModel
{
    public List<Application> Applications { get; set; }

    public void OnGet()
    {

        Applications = ApplicationRepo.Applications;
    }
    [BindProperty]
    public int ApplicationId { get; set; }

    [BindProperty]
    public string Note { get; set; }
    [BindProperty]
    public bool Approved { get; set; }


    public IActionResult OnPostUpdateNote()
    {
        var appToEdit = ApplicationRepo.Applications.FirstOrDefault(a => a.ApplicationId == ApplicationId);

        if (appToEdit != null)
        {
            appToEdit.Note = Note;
        }

        return RedirectToPage(); // reloads the same page
    }
    public IActionResult OnPostToggleApproval()
    {
        var appToUpdate = ApplicationRepo.Applications
            .FirstOrDefault(a => a.ApplicationId == ApplicationId);

        if (appToUpdate != null)
        {
            appToUpdate.Approved = Approved;
        }

        return RedirectToPage();
    }



}
