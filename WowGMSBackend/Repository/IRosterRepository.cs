using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.Repository
{
    public interface IRosterRepository
    {
        IEnumerable<BossRoster> GetAll();
        BossRoster? GetById(int id);
        BossRoster Add(BossRoster roster);
        BossRoster? Update(BossRoster updated);
        BossRoster? Delete(int id);
        void MarkAsProcessed(int rosterId);
        void AddParticipant(int rosterId, Character character);
        void RemoveParticipant(int rosterId, int characterId);
    }
}
