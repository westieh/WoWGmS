using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Model;

namespace WowGMSBackend.Model
{
    public class Apply
    {
        public int ApplicationId { get; set; }
        public string? CharacterName { get; set; }
        public string? DiscordName { get; set; }
        public string? Password { get; set; }
        public Class Class { get; set; }
        public Role Role { get; set; }
        public ServerName ServerName { get; set; }
    }
}
