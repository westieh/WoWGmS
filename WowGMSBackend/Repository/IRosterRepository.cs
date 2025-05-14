using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Model;

namespace WowGMSBackend.Repository
{
    public interface IRosterRepository
    {
        IEnumerable<BossRoster> GetAll();
        void MarkAsProcessed(int rosterId);
    }
}
