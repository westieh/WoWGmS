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

    // List of pending applications
    public List<Application> Applications { get; set; } = new();

    // Constructor injecting application, member, and character services
    public ViewApplicationsModel(IApplicationService applicationService, ICharacterService characterService, IMemberService memberService)
    {
        _applicationService = applicationService;
        _characterService = characterService;
        _memberService = memberService;
    }

    // Handles GET requests to load pending applications
    public void OnGet()
    {
        Applications = _applicationService.GetPendingApplications(); // filters for !Approved
    }

    // Bound property for application ID (used in form posts)
    [BindProperty]
    public int ApplicationId { get; set; }

    // Bound property for note input (used in form posts)
    [BindProperty]
    public string Note { get; set; }

    // Bound property for approval status (used in form posts)
    [BindProperty]
    public bool Approved { get; set; }

    // Handles POST request to update an application's note
    public IActionResult OnPostUpdateNote()
    {
        var application = _applicationService.GetApplicationById(ApplicationId);
        if (application == null)
        {
            TempData["Error"] = "Application not found.";
            return RedirectToPage();
        }

        if (!string.IsNullOrWhiteSpace(Note))
        {
            _applicationService.AppendToNote(ApplicationId, Note.Trim());
            TempData["Success"] = "Note successfully appended.";
        }
        else
        {
            TempData["Error"] = "Note input was empty.";
        }

        return RedirectToPage();
    }

    // Handles POST request to toggle approval of an application
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
                // Approve the application
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
                // Revoke approval
                appToUpdate.Approved = false;
                appToUpdate.ProcessedBy = null;

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
