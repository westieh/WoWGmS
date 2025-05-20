using System.Collections.Generic;
using WoW.Model;

namespace WowGMSBackend.Service
{
    public interface IApplicationService
    {
        List<Application> GetAllApplications();
        Application? GetApplicationById(int id);
        void AddApplication(Application application);
        bool UpdateApplication(Application application);
        bool DeleteApplication(int id);
        void ApproveApplication(Application application);
    }
}