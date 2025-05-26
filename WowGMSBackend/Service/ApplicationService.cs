using System.Linq;
using System;
using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;

namespace WowGMSBackend.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly MemberService _memberService;
        private readonly ApplicationRepo _applicationRepo;
        private readonly CharacterService _characterService;

        public ApplicationService(MemberService memberService, ApplicationRepo applicationRepo, CharacterService characterService)
        {
            _memberService = memberService;
            _applicationRepo = applicationRepo;
            _characterService = characterService;
        }

        public void AddApplication(Application application)
        {
            _applicationRepo.AddApplication(application);
        }

        public void ApproveApplication(Application application)
        {
            if (application.Approved) return;

            application.Approved = true;
            application.SubmissionDate = DateTime.Now;


            var existingMembers = _memberService.GetMembers();
            bool alreadyMember = existingMembers.Any(m => m.Name == application.DiscordName);
            if (!alreadyMember)
            {
                var newMember = new Member
                {
                    Name = application.DiscordName!,
                    Password = application.Password!,
                    Rank = Rank.Trialist
                };
                _memberService.AddMember(newMember);
                var newCharacter = new Character
                {
                    CharacterName = application.CharacterName!,
                    Class = application.Class,
                    Role = application.Role,
                    MemberId = newMember.MemberId
                };
            }

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
