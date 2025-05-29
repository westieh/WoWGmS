using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.ViewModels
{
    public class MemberPanelViewModel
    {
        public List<Boss> AllBosses { get; set; }
        public List<Character> Characters { get; set; }
        public string SelectedBossSlug { get; set; }
    }
}
