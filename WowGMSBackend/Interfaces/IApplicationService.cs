using System.Collections.Generic;
using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
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
