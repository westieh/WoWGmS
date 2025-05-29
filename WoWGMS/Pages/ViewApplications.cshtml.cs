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
        try
        {
            var appToUpdate = _applicationService.GetApplicationById(ApplicationId);

            if (appToUpdate == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToPage();
            }

            if (Approved && !appToUpdate.Approved)
            {
                var officer = _memberService.GetMemberByName(User.Identity?.Name);
                if (officer == null)
                {
                    TempData["Error"] = "Officer not recognized.";
                    return RedirectToPage();
                }

                appToUpdate.ProcessedBy = officer;
                appToUpdate.Note = Note;

                _applicationService.ApproveApplication(appToUpdate);
                TempData["Success"] = "Application approved and member created.";
            }
            else if (!Approved && appToUpdate.Approved)
            {
                appToUpdate.Approved = false;
                appToUpdate.ProcessedBy = null;

                // You might want to persist this change if applicable:
                _applicationService.ApproveApplication(appToUpdate);

                TempData["Success"] = "Application approval revoked.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToPage();
    }

}
