using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoW.Model;
using WowGMSBackend.Service;  // use service namespace

public class CreateApplicationModel : PageModel
{
    private readonly IApplicationService _applicationService;

    public CreateApplicationModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [BindProperty]
    public Application Application { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Application.SubmissionDate = DateTime.Now;
        Application.Approved = false;

        _applicationService.SubmitApplication(Application);

        return RedirectToPage("/Index");
    }
}
