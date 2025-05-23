using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Collections.Generic;
using WoW.Model;
using WoWGMS.Repository;
using WoWGMS.Service;
using WowGMSBackend.MockData;
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

        Applications = _applicationService.GetAllApplications();
        return Page();
    }

}
