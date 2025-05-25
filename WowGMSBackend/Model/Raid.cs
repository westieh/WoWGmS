using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowGMSBackend.Model
{
    public class Raid
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public List<Boss> Bosses { get; set; } = new();
    }
}
