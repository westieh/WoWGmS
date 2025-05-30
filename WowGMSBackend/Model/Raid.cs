using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowGMSBackend.Model
{
    // Represents a raid, containing multiple bosses
    public class Raid
    {
        // Name of the raid (e.g., "Aberrus, the Shadowed Crucible")
        public string Name { get; set; }

        // Slugified name of the raid for URL-friendly usage (e.g., "aberrus")
        public string Slug { get; set; }

        // List of bosses present in this raid
        public List<Boss> Bosses { get; set; } = new();
    }
}
