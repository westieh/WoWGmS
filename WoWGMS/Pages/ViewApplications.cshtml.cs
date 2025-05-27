using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "Officer")]
public class ViewApplicationsModel : PageModel
{
    private readonly IApplicationService _applicationService;
    private readonly IMemberService _memberService;
    private readonly ICharacterService _characterService;
    public List<Application> Applications { get; set; } = new();
    public ViewApplicationsModel(IApplicationService applicationService, IMemberService memberService, ICharacterService characterService)
    {
        _applicationService = applicationService;
        _memberService = memberService;
        _characterService = characterService;
    }
    public void OnGet()
    {
        Applications = _applicationService
            .GetPendingApplications(); // already filters for !Approved
    }
    [BindProperty]
    public int ApplicationId { get; set; }

    [BindProperty]
    public string Note { get; set; }
    [BindProperty]
    public bool Approved { get; set; }


    
    public IActionResult OnPostToggleApproval()
    {
        var appToUpdate = _applicationService.GetApplicationById(ApplicationId);

        if (appToUpdate != null)
        {
            if (Approved && !appToUpdate.Approved)
            {
                // Approve the application
                // Member Created via Applicationservice
                
                _applicationService.ApproveApplication(appToUpdate);

                
            }
            else if (!Approved && appToUpdate.Approved)
            {
                // Un-approve if needed (optional logic)
                appToUpdate.Approved = false;
            }
        }

        return RedirectToPage();
    }

}
