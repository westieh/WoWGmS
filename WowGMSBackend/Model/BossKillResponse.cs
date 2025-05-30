using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowGMSBackend.Model
{
    /// <summary>
    /// Represents the response from the Raider.IO API used by the BossKillCheckerService.
    /// </summary>
    public class BossKillResponse
    {
        public KillData Kill { get; set; }
    }

    /// <summary>
    /// Contains kill details from the API response.
    /// </summary>
    public class KillData
    {
        public DateTime DefeatedAt { get; set; }
        public bool IsSuccess { get; set; }
    }
}
