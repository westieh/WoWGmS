using System.Collections.Generic;
using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public interface IApplicationService
    {
        void ApproveApplication(Application application);
        void SubmitApplication(Application application);
        List<Application> GetAllApplications();
        List<Application> GetPendingApplications();
        void AddApplication(Application application);
        Application? GetApplicationById(int id);
    }
}
