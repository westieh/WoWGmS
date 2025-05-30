using System.Collections.Generic;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WowGMSBackend.Repository
{
    /// <summary>
    /// Repository responsible for CRUD operations on Application entities.
    /// </summary>
    public class ApplicationRepo : IApplicationRepo
    {
        private readonly WowDbContext _context;

        public ApplicationRepo(WowDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all applications from the database, including associated boss kills and processor information.
        /// </summary>
        public List<Application> GetApplications()
        {
            return _context.Applications
                .Include(a => a.BossKills)      // Eager-load BossKill records linked to the application
                .Include(a => a.ProcessedBy)    // Eager-load the Member who processed the application
                .ToList();
        }

        /// <summary>
        /// Retrieves a single application by ID, including its boss kills.
        /// </summary>
        /// <param name="id">Application ID</param>
        public Application? GetApplicationById(int id)
        {
            return _context.Applications
                .Include(a => a.BossKills)  // Ensure related boss kills are loaded to avoid lazy-loading issues
                .FirstOrDefault(a => a.ApplicationId == id);
        }

        /// <summary>
        /// Adds a new application to the database and saves changes.
        /// </summary>
        /// <param name="application">Application entity to add</param>
        public void AddApplication(Application application)
        {
            _context.Applications.Add(application);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing application with new values. Only updates specific fields.
        /// </summary>
        /// <param name="updatedApplication">Application entity containing updated data</param>
        /// <returns>True if the application was found and updated; otherwise, false.</returns>
        public bool UpdateApplication(Application updatedApplication)
        {
            var existing = _context.Applications.FirstOrDefault(a => a.ApplicationId == updatedApplication.ApplicationId);
            if (existing == null) return false;

            // Update allowed fields
            existing.CharacterName = updatedApplication.CharacterName;
            existing.DiscordName = updatedApplication.DiscordName;
            existing.Class = updatedApplication.Class;
            existing.Role = updatedApplication.Role;
            existing.Note = updatedApplication.Note;
            existing.Approved = updatedApplication.Approved;
            existing.ProcessedBy = updatedApplication.ProcessedBy;
            existing.SubmissionDate = updatedApplication.SubmissionDate;

            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes an application by its ID.
        /// </summary>
        /// <param name="id">Application ID to delete</param>
        /// <returns>True if the application was found and deleted; otherwise, false.</returns>
        public bool DeleteApplication(int id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationId == id);
            if (application == null) return false;

            _context.Applications.Remove(application);
            _context.SaveChanges();
            return true;
        }
    }
}
