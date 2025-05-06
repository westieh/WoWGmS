using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WowGMSBackend.MockData;

namespace WoWGMS.Pages.Admin
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Username == MockAdmin.Username && Password == MockAdmin.Password)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, MockAdmin.Username),
                    new Claim(ClaimTypes.Role, MockAdmin.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return RedirectToPage("/Index");
            }
            ModelState.AddModelError("", "Forkert brugernavn eller adgangskode.");
            return Page();
        }

        public void OnGet()
        {
        }
    }
}
