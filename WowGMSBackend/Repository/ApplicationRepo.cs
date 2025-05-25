using System.Collections.Generic;
using WowGMSBackend.DBContext;
using WowGMSBackend.MockData;
using WowGMSBackend.Model;

namespace WowGMSBackend.Repository
{
    public class ApplicationRepo
    {
        private readonly WowDbContext _context;

        public ApplicationRepo(WowDbContext context)
        {
            _context = context;
        }

        public List<Application> GetApplications()
        {
            return _context.Applications.ToList();
        }

        public Application? GetApplicationById(int id)
        {
            return _context.Applications.FirstOrDefault(a => a.ApplicationId == id);
        }

        public void AddApplication(Application application)
        {
            _context.Applications.Add(application);
            _context.SaveChanges();
        }

        public bool UpdateApplication(Application updatedApplication)
        {
            var existing = _context.Applications.FirstOrDefault(a => a.ApplicationId == updatedApplication.ApplicationId);
            if (existing == null) return false;

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
