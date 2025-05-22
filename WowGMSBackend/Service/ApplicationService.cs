using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public class ApplicationService : IApplicationService
    {
        public ApplicationRepo _applicationRepo { get; private set; }

        public ApplicationService(ApplicationRepo applicationRepo)
        {
            _applicationRepo = applicationRepo;
        }

        public List<Application> GetAllApplications()
        {
            return _applicationRepo.GetApplications();
        }

        public Application? GetApplicationById(int id)
        {
            return _applicationRepo.GetApplicationById(id);
        }

        public void AddApplication(Application application)
        {
            _applicationRepo.AddApplication(application);
        }

        public bool UpdateApplication(Application application)
        {
            return _applicationRepo.UpdateApplication(application);
        }

        public bool DeleteApplication(int id)
        {
            return _applicationRepo.DeleteApplication(id);
        }

        public void ApproveApplication(Application application)
        {
            application.Approved = true;
            _applicationRepo.UpdateApplication(application);
        }
    }
}