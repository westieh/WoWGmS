using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using Microsoft.EntityFrameworkCore;
using WowGMSBackend.DBContext;
namespace WowGMSBackend.Repository
{
    public class BossKillRepo : IBossKillRepo
    {
        private readonly WowDbContext _context;

        public BossKillRepo(WowDbContext context)
        {
            _context = context;
        }


        public void AddBossKill(BossKill bossKill)
        {
            _context.BossKills.Add(bossKill);
            _context.SaveChanges();
        }

        public void DeleteBossKillsForCharacter(int characterId)
        {
            var kills = _context.BossKills.Where(k => k.CharacterId == characterId).ToList();
            _context.BossKills.RemoveRange(kills);
            _context.SaveChanges();
        }

        public List<BossKill> GetBossKillsByCharacterId(int characterId)
        {
            return _context.BossKills
                .Where(k => k.CharacterId == characterId)
                .ToList();
        }
    }
}