using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.MockData;
using WoW.Model;

namespace WowGMSBackend.Repository
{
    public class BossRosterRepo
    {
        private static List<BossRoster> _bossRosters = MockBossRoster.GetBossRoster();
        private static int _nextId = 1;

        public List<BossRoster> GetAll()
        {
            return _bossRosters;
        }

        public BossRoster? GetById(int id)
        {
            return _bossRosters.FirstOrDefault(r => r.RosterId == id);
        }

        public BossRoster Add(BossRoster roster)
        {
            roster.RosterId = _nextId++;
            roster.CreationDate = DateTime.Now;
            _bossRosters.Add(roster);
            return roster;
        }

        public BossRoster? Update(BossRoster updated)
        {
            var existing = GetById(updated.RosterId);
            if (existing != null)
            {
                existing.BossName = updated.BossName;
                existing.InstanceTime = updated.InstanceTime;
                existing.IsProcessed = updated.IsProcessed;
                return existing;
            }
            return null;
        }

        public BossRoster? Delete(int id)
        {
            var roster = GetById(id);
            if (roster != null)
            {
                _bossRosters.Remove(roster);
                return roster;
            }
            return null;
        }
    }
}
