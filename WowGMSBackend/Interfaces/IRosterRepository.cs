using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface IRosterRepository
    {
        IEnumerable<BossRoster> GetAll();
        BossRoster? GetById(int id);
        BossRoster Add(BossRoster roster);
        BossRoster Update(BossRoster roster);
        BossRoster? Delete(int id);


    }
}
