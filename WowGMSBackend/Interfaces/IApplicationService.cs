using System.Collections.Generic;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface IApplicationService
    {
        void ApproveApplication(Application application);
        void SubmitApplication(Application application, Dictionary<string, int> bossKills);
        List<Application> GetAllApplications();
        List<Application> GetPendingApplications();
        void AddApplication(Application application);
        Application? GetApplicationById(int id);
        void AppendToNote(int applicationId, string additionalNote, string? author = null);
        string? GetNoteByApplicationId(int applicationId);

    }
}
