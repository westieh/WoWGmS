using System;
using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly MemberService _memberService;
        private readonly ApplicationRepo _applicationRepo;

        public ApplicationService(MemberService memberService, ApplicationRepo applicationRepo)
        {
            _memberService = memberService;
            _applicationRepo = applicationRepo;
        }

        public void AddApplication(Application application)
        {
            _applicationRepo.AddApplication(application);
        }

        public void ApproveApplication(Application application)
        {
            if (application.Approved) return;

            var newMember = new Member
            {
                MemberId = _memberService.GenerateNextMemberId(),
                Name = application.DiscordName,
                Password = application.Password,
                Rank = Rank.Trialist
            };

            _memberService.AddMember(newMember);
            application.Approved = true;
            application.SubmissionDate = DateTime.Now;

            _applicationRepo.UpdateApplication(application);
        }

        public List<Application> GetPendingApplications()
        {
            return _applicationRepo.GetApplications().Where(a => !a.Approved).ToList();
        }

        public void SubmitApplication(Application application)
        {
            application.SubmissionDate = DateTime.Now;
            _applicationRepo.AddApplication(application);
        }

        public List<Application> GetAllApplications()
        {
            return _applicationRepo.GetApplications();
        }

        public Application? GetApplicationById(int id)
        {
            return _applicationRepo.GetApplicationById(id);
        }

        public bool DeleteApplication(int id)
        {
            return _applicationRepo.DeleteApplication(id);
        }
    }
}
