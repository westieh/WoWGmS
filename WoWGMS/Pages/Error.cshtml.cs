using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WoWGMS.Pages
{
    // Disables response caching and antiforgery validation for the error page
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        // Stores the request ID for error tracking
        public string? RequestId { get; set; }

        // Indicates whether the request ID should be shown
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        // Constructor injecting the logger
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        // Handles GET requests and assigns the request ID
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
