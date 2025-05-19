using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowGMSBackend.Model
{
    public class BossKillResponse
    {
        public KillData Kill { get; set; }
    }

    public class KillData
    {
        public DateTime DefeatedAt { get; set; }
        public bool IsSuccess { get; set; }
    }
}
