using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WowGMSBackend.MockData;
using WowGMSBackend.Service;

namespace WoWGMS.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly IMemberService _memberService;
        public LoginModel(IMemberService memberService)
        {
            _memberService = memberService;
        }
        [BindProperty] public string? Username { get; set; }
        [BindProperty] public string? Password { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = _memberService.ValidateLogin(Username!, Password!);
            if (user != null)
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
