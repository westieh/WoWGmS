using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WoWGMS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        // Constructor injecting the logger
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        // Handles GET requests for the index page
        public void OnGet()
        {
            // No specific logic needed for the homepage GET request
        }
    }
}
