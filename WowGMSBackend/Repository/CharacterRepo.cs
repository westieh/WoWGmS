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
            return _context.Characters.Include(c => c.Member).FirstOrDefault(c => c.Id == id);
        }

        public List<Character> GetCharacters()
        {
            return _context.Characters.Include(c => c.Member).ToList();
        }

        public Character? UpdateCharacter(int id, Character updated)
        {
            var existing = _context.Characters.FirstOrDefault(c => c.Id == id);
            if (existing != null)
            {
                existing.CharacterName = updated.CharacterName;
                existing.RealmName = updated.RealmName;
                existing.Class = updated.Class;
                existing.Role = updated.Role;
                existing.MemberId = updated.MemberId;
                existing.Member = updated.Member;

                _context.SaveChanges();
                return existing;
            }
            return null;
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
            return _context.Characters.Where(c => c.MemberId == memberId).ToList();
        }
    }
}
