using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
namespace WowGMSBackend.Repository
{
    public class CharacterRepo : ICharacterRepo
    {
        private readonly WowDbContext _context;
        

        public CharacterRepo(WowDbContext context)
        {
            _context = context ;
        }

        public Character AddCharacter(Character character)
        {
            _context.Characters.Add(character);

            _context.SaveChanges();
            return character;
        }

        public Character? GetCharacter(int id)
        {
            return _context.Characters
                .Include(c => c.BossKills)  // ✅ This is critical
                .FirstOrDefault(c => c.Id == id);
        }
        public void AddBossKill(BossKill bossKill)
        {
            _context.BossKills.Add(bossKill);
            _context.SaveChanges();
        }
        public List<Character> GetCharacters()
        {
            return _context.Characters
         .Include(c => c.Member)
         .Include(c => c.BossKills)
         .ToList();
        }

        public Character? UpdateCharacter(int id, Character updated)
        {
            var existing = _context.Characters.FirstOrDefault(c => c.Id == id);
            if (existing == null) return null;

            existing.CharacterName = updated.CharacterName;
            existing.RealmName = updated.RealmName;
            existing.Class = updated.Class;
            existing.Role = updated.Role;
            existing.MemberId = updated.MemberId;
            existing.Member = updated.Member;
            _context.SaveChanges();
            return existing;
        }


        public Character? DeleteCharacter(int id)
        {
            var character = _context.Characters.FirstOrDefault(c => c.Id == id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                _context.SaveChanges();
                return character;
            }
            return null;
        }

        public List<Character> GetCharactersByMemberId(int memberId)
        {
            return _context.Characters
                .Include(c => c.BossKills) // ✅ ensures kill data is available
                .Where(c => c.MemberId == memberId)
                .ToList();
        }


        public List<Character> GetAllCharacters()
        {
            return _context.Characters.AsNoTracking().ToList();
        }
        public List<Character> GetCharactersByRoster(int rosterId)
        {
            return _context.Characters
                .Include(c => c.BossKills)
                .Where(c => _context.BossRosters
                    .Where(r => r.RosterId == rosterId)
                    .SelectMany(r => r.Participants)
                    .Contains(c))
                .ToList();
        }



    }
}
