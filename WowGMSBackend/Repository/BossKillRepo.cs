using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using Microsoft.EntityFrameworkCore;
using WowGMSBackend.DBContext;

namespace WowGMSBackend.Repository
{
    /// <summary>
    /// Repository for performing CRUD operations on BossKill entities.
    /// </summary>
    public class BossKillRepo : IBossKillRepo
    {
        private readonly WowDbContext _context;

        public BossKillRepo(WowDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new BossKill record to the database.
        /// </summary>
        public void AddBossKill(BossKill bossKill)
        {
            _context.BossKills.Add(bossKill);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes all BossKill records associated with a specific character.
        /// </summary>
        public void DeleteBossKillsForCharacter(int characterId)
        {
            var kills = _context.BossKills
                .Where(k => k.CharacterId == characterId)
                .ToList();
            _context.BossKills.RemoveRange(kills);
            _context.SaveChanges();
        }

        /// <summary>
        /// Retrieves all BossKill records for a specific character.
        /// </summary>
        public List<BossKill> GetBossKillsByCharacterId(int characterId)
        {
            return _context.BossKills
                .Where(k => k.CharacterId == characterId)
                .ToList();
        }
    }
}
