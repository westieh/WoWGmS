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

        // Binds the username input from the form
        [BindProperty]
        public string? Username { get; set; }

        // Binds the password input from the form
        [BindProperty]
        public string? Password { get; set; }

        // Stores error messages to display on the page
        public string? ErrorMessage { get; set; }

        // Constructor injecting the member service
        public LoginModel(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // Handles the POST request for login
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validate if username or password fields are empty
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Username and password are required.";
                    return Page();
                }

                // Attempt to validate the login credentials
                var member = _memberService.ValidateLogin(Username, Password);

                if (member != null)
                {
                    // Create claims based on the authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, member.Name),
                        new Claim(ClaimTypes.Role, member.Rank.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, member.MemberId.ToString()),
                        new Claim("MemberId", member.MemberId.ToString())
                    };

                    // Create a claims identity with the cookie authentication scheme
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in the user with the cookie authentication
                    await HttpContext.SignInAsync("MyCookieAuth", principal);

                    // Redirect to the home page on successful login
                    return RedirectToPage("/Index");
                }

                // Set error message for invalid login
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
            catch (Exception ex)
            {
                // Handle unexpected errors during login
                ErrorMessage = "An error occurred during login. Please try again.";
                // Log ex.Message if needed
                return Page();
            }
        }
    }
}
