using Microsoft.AspNetCore.Mvc;
using System;
using WowGMSBackend.Model;
using WowGMSBackend.DBContext;
namespace WoWGMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly WowDbContext _context;

        public ApplicationController(WowDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ReceiveApplication([FromBody] RawApplicationDto raw)
        {
            try
            {
                if (!Enum.TryParse<ServerName>(Sanitize(raw.ServerName), true, out var serverName) ||
                    !Enum.TryParse<Class>(Sanitize(raw.Class), true, out var charClass) ||
                    !Enum.TryParse<Role>(Sanitize(raw.MainRole), true, out var mainRole))
                {
                    return BadRequest(new { error = "Invalid enum values" });
                }

                var application = new Application
                {
                    DiscordName = raw.DiscordName ?? string.Empty,
                    CharacterName = raw.CharacterName ?? string.Empty,
                    ServerName = serverName,
                    Class = charClass,
                    Role = mainRole,
                    SubmissionDate = DateTime.Now
                };

                _context.Applications.Add(application);
                _context.SaveChanges();

                return Ok(new { status = "Application saved" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Data error", details = ex.Message });
            }
        }

        private string Sanitize(string? input)
        {
            return (input ?? string.Empty)
                   .Replace(" ", "")
                   .Replace("-", "")
                   .Replace("’", "")
                   .Replace("'", "")
                   .Trim();
        }
    }

    public class RawApplicationDto
    {
        public string? DiscordName { get; set; }
        public string? CharacterName { get; set; }
        public string? ServerName { get; set; }
        public string? Class { get; set; }
        public string? MainRole { get; set; }
    }
}