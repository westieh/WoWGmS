using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Service;

public class ViewApplicationsModel : PageModel
{
    private readonly IApplicationService _applicationService;
    private readonly IMemberService _memberService;
    private readonly ICharacterService _characterService;

    public ViewApplicationsModel(IApplicationService applicationService, IMemberService memberService, ICharacterService characterService)
    {
        _applicationService = applicationService;
        _memberService = memberService;
        _characterService = characterService;
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

        if (application != null)
        {
            if (Approved && !application.Approved)
            {
                // Approve the application
                _applicationService.ApproveApplication(application);

                // Check if member already exists
                var existingMembers = _memberService.GetMembers();
                bool alreadyMember = existingMembers.Any(m => m.Name == application.DiscordName);

                if (!alreadyMember)
                {
                    // Add the member
                    var newMember = _memberService.AddMember(new Member
                    {
                        Name = application.DiscordName,
                        Password = application.Password,
                        Rank = Rank.Trialist
                    });

                    // Add the character for that member
                    _characterService.AddCharacter(new Character
                    {
                        MemberId = newMember.MemberId, // Link character to member
                        CharacterName = application.CharacterName,
                        RealmName = application.ServerName,
                        Class = application.Class,
                        Role = application.Role
                    });
                }
            }
            else if (!Approved && application.Approved)
            {
                // Un-approve if needed (optional logic)
                application.Approved = false;
            }
        }

        return RedirectToPage();
    }

}
