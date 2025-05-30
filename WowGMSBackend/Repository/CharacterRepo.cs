using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;

namespace WowGMSBackend.Repository
{
    /// <summary>
    /// Repository for managing Character entities in the database.
    /// </summary>
    public class CharacterRepo : ICharacterRepo
    {
        private readonly WowDbContext _context;

        public CharacterRepo(WowDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new character to the database.
        /// </summary>
        public Character AddCharacter(Character character)
        {
            _context.Characters.Add(character);
            _context.SaveChanges();
            return character;
        }

        /// <summary>
        /// Retrieves a character by its ID including its BossKills.
        /// </summary>
        public Character? GetCharacter(int id)
        {
            return _context.Characters
                .Include(c => c.BossKills)
                .FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Adds a BossKill entry for a character.
        /// </summary>
        public void AddBossKill(BossKill bossKill)
        {
            _context.BossKills.Add(bossKill);
            _context.SaveChanges();
        }

        /// <summary>
        /// Retrieves all characters with related Member and BossKills data.
        /// </summary>
        public List<Character> GetCharacters()
        {
            return _context.Characters
                .Include(c => c.Member)
                .Include(c => c.BossKills)
                .ToList();
        }

        /// <summary>
        /// Updates an existing character's information.
        /// </summary>
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

        /// <summary>
        /// Deletes a character from the database.
        /// </summary>
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

        /// <summary>
        /// Retrieves all characters associated with a specific member.
        /// </summary>
        public List<Character> GetCharactersByMemberId(int memberId)
        {
            return _context.Characters
                .Include(c => c.BossKills)
                .Where(c => c.MemberId == memberId)
                .ToList();
        }

        /// <summary>
        /// Retrieves all characters without tracking (read-only).
        /// </summary>
        public List<Character> GetAllCharacters()
        {
            return _context.Characters.AsNoTracking().ToList();
        }

        /// <summary>
        /// Retrieves all characters participating in a specific roster.
        /// </summary>
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
