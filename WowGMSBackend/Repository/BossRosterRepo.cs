using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;
using WowGMSBackend.MockData;
using WowGMSBackend.Model;

namespace WowGMSBackend.Repository
{
    public class BossRosterRepo : IRosterRepository
    {
        private readonly WowDbContext _context;
        public BossRosterRepo(WowDbContext context)
        {
            _context = context;
        }

        public IEnumerable<BossRoster> GetAll()
        {
            return _context.BossRosters.Include(r => r.Participants).ToList();
        }
        public void MarkAsProcessed(int rosterId)
        {
            var roster = _context.BossRosters.Include(r => r.Participants).FirstOrDefault(r => r.RosterId == rosterId);
            if (roster == null || roster.IsProcessed)
                return;

            var boss = roster.GetBoss();
            if (boss == null) return;

            foreach (var character in roster.Participants)
            {
                character.IncrementBossKill(boss.Slug);
            }

            roster.IsProcessed = true;
        }
        public BossRoster? GetById(int id)
        {
            return _context.BossRosters
            .Include(r => r.Participants)
            .FirstOrDefault(r => r.RosterId == id);
        }

        public BossRoster Add(BossRoster roster)
        {
            roster.CreationDate = DateTime.Now;
            _context.BossRosters.Add(roster);
            _context.SaveChanges();
            return roster;
        }

        public BossRoster? Update(BossRoster updated)
        {
            var existing = _context.BossRosters.Find(updated.RosterId);
            if (existing != null)
            {
                existing.RaidSlug = updated.RaidSlug;
                existing.BossDisplayName = updated.BossDisplayName;
                existing.InstanceTime = updated.InstanceTime;
                existing.IsProcessed = updated.IsProcessed;
                return existing;
            }
            return null;
        }

        public BossRoster? Delete(int id)
        {
            var roster = _context.BossRosters.Find(id);
            if (roster != null)
            {
                _context.BossRosters.Remove(roster);
                _context.SaveChanges();
                return roster;
            }
            return null;
        }
    }
}
