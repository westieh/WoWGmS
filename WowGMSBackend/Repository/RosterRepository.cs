﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;

namespace WowGMSBackend.Repository
{
    /// <summary>
    /// Repository for managing BossRoster entities.
    /// </summary>
    public class RosterRepository : IRosterRepository
    {
        private readonly WowDbContext _context;

        public RosterRepository(WowDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all rosters including their participants.
        /// </summary>
        public IEnumerable<BossRoster> GetAll()
        {
            return _context.BossRosters.Include(r => r.Participants).ToList();
        }

        /// <summary>
        /// Retrieves a specific roster by its ID.
        /// </summary>
        public BossRoster? GetById(int id)
        {
            return _context.BossRosters
                .Include(r => r.Participants)
                .FirstOrDefault(r => r.RosterId == id);
        }

        /// <summary>
        /// Adds a new roster to the database.
        /// </summary>
        public BossRoster Add(BossRoster roster)
        {
            _context.BossRosters.Add(roster);
            _context.SaveChanges();
            return roster;
        }

        /// <summary>
        /// Updates an existing roster.
        /// </summary>
        public BossRoster? Update(BossRoster updated)
        {
            var existing = _context.BossRosters
                .Include(r => r.Participants)
                .ThenInclude(c => c.BossKills)
                .FirstOrDefault(r => r.RosterId == updated.RosterId);

            if (existing != null)
            {
                existing.RaidSlug = updated.RaidSlug;
                existing.BossDisplayName = updated.BossDisplayName;
                existing.InstanceTime = updated.InstanceTime;
                existing.IsProcessed = updated.IsProcessed;

                _context.SaveChanges();
                return existing;
            }

            return null;
        }

        /// <summary>
        /// Deletes a roster by its ID.
        /// </summary>
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
