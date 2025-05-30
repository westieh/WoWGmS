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
        private readonly IMemberService _memberService;
        private readonly IApplicationRepo _applicationRepo;
        private readonly ICharacterService _characterService;
        private readonly IBossKillService _bossKillService;

        public ApplicationService(IMemberService memberService, IApplicationRepo applicationRepo, ICharacterService characterService, IBossKillService bossKillService)
        {
            _memberService = memberService;
            _applicationRepo = applicationRepo;
            _characterService = characterService;
            _bossKillService = bossKillService;
        }

        public void AddApplication(Application application)
        {
            _applicationRepo.AddApplication(application);
        }
        public void AppendToNote(int applicationId, string additionalNote, string? author = null)
        {
            var app = _applicationRepo.GetApplicationById(applicationId);
            if (app == null) throw new Exception("Application not found.");

            if (string.IsNullOrWhiteSpace(additionalNote)) return;

            var noteEntry = additionalNote.Trim();
            if (!string.IsNullOrEmpty(author))
            {
                noteEntry = $"[{author}] {noteEntry}";
            }

            var separator = string.IsNullOrEmpty(app.Note) ? "" : " | ";
            app.Note += $"{separator}{noteEntry}";

            _applicationRepo.UpdateApplication(app);
        }
        public string? GetNoteByApplicationId(int applicationId)
        {
            var app = _applicationRepo.GetApplicationById(applicationId);
            if (app == null) throw new Exception("Application not found.");
            return app.Note;
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
                    MemberId = newMember.MemberId,
                    RealmName = application.ServerName
                };

                _characterService.AddCharacter(newCharacter); // Character now has an ID

                // ✅ Delegate boss kill persistence to the BossKillService
                _bossKillService.TransferFromApplication(application, newCharacter.Id);
            }

            _applicationRepo.UpdateApplication(application);
        }


        public List<Application> GetPendingApplications()
        {
            return _applicationRepo.GetApplications().Where(a => !a.Approved).ToList();
        }

        public void SubmitApplication(Application application, Dictionary<string, int> bossKills)
        {
            application.Approved = false;

            application.ProcessedBy = null;
            application.SubmissionDate = DateTime.Now;

            application.BossKills = bossKills
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => new BossKill
                {
                    BossSlug = kvp.Key,
                    KillCount = kvp.Value,
                    Application = application
                })
                .ToList();

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
