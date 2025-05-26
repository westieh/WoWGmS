using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;
namespace WowGMSBackend.Interfaces
{
    public interface IApplicationRepo
    {
        List<Application> GetApplications();
        Application? GetApplicationById(int id);
        void AddApplication(Application application);
        bool UpdateApplication(Application updatedApplication);
        bool DeleteApplication(int id);

    }
}
