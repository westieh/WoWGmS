using System.Collections.Generic;
using System.Linq;
using WoW.MockData;
using WoW.Model;

namespace WoWGMS.Repository
{
    public class ApplicationRepo
    {
        private readonly List<Application> _applications;

        public ApplicationRepo()
        {
            _applications = MockApplications.Get();  // Initialize with mock data
        }

        public List<Application> GetApplications()
        {
            return _applications;
        }

        public Application? GetApplicationById(int id)
        {
            return _applications.FirstOrDefault(a => a.ApplicationId == id);
        }

        public void AddApplication(Application application)
        {
            _applications.Add(application);
        }

        public bool UpdateApplication(Application updatedApplication)
        {
            var existing = _applications.FirstOrDefault(a => a.ApplicationId == updatedApplication.ApplicationId);
            if (existing == null) return false;

            existing.CharacterName = updatedApplication.CharacterName;
            existing.DiscordName = updatedApplication.DiscordName;
            existing.Class = updatedApplication.Class;
            existing.Role = updatedApplication.Role;
            existing.Note = updatedApplication.Note;
            existing.Approved = updatedApplication.Approved;
            existing.ProcessedBy = updatedApplication.ProcessedBy;
            existing.SubmissionDate = updatedApplication.SubmissionDate;
            return true;
        }

        public bool DeleteApplication(int id)
        {
            var application = _applications.FirstOrDefault(a => a.ApplicationId == id);
            if (application == null) return false;
            _applications.Remove(application);
            return true;
        }
    }
}
