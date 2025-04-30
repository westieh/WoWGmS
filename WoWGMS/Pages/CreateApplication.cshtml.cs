using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WoW.Model; // tilføj dette hvis Application-modellen er her

public class CreateApplicationModel : PageModel
{
    [BindProperty]
    public Application Application { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Application.SubmissionDate = DateTime.Now;
        Application.Approved = false;

        ApplicationRepo.Applications.Add(Application);

        return RedirectToPage("/Index");
    }
}
