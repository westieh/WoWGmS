using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;
using WowGMSBackend.Service;
using Microsoft.Identity.Client;
using WowGMSBackend.Interfaces;
namespace WoWGMS.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly IMemberService _memberService;

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public LoginModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var member = _memberService.ValidateLogin(Username, Password);

            if (member != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, member.Name),
                    new Claim(ClaimTypes.Role, member.Rank.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, member.MemberId.ToString()),
                    new Claim("MemberId", member.MemberId.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                // Redirect to whatever page you want after login


                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid username or password.";
            return Page();
        }
    }
}
