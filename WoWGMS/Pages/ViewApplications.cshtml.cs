using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;

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

        Applications = _applicationService.GetAllApplications();
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
                _applicationService.ApproveApplication(appToUpdate);

                // Check if member already exists
                var existingMembers = _memberService.GetMembers();
                bool alreadyMember = existingMembers.Any(m => m.Name == appToUpdate.DiscordName);

                if (!alreadyMember)
                {
                    // Add the member
                    var newMember = _memberService.AddMember(new Member
                    {
                        Name = appToUpdate.DiscordName,
                        Password = appToUpdate.Password,
                        Rank = Rank.Trialist
                    });
                    

                    // Add the character for that member
                    _characterService.AddCharacter(new Character
                    {
                        MemberId = newMember.MemberId, // Link character to member
                        CharacterName = appToUpdate.CharacterName,
                        RealmName = appToUpdate.ServerName,
                        Class = appToUpdate.Class,
                        Role = appToUpdate.Role
                    });
                }
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
