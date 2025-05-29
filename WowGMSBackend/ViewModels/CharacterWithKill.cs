using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.ViewModels
{
    public class CharacterWithKill
    {
        public Character Character { get; set; }
        public int KillCount { get; set; }
    }
}
