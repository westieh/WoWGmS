using System.Linq;
using System;
using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;

namespace WowGMSBackend.Service
{
    /// <summary>
    /// Service layer handling business logic for applications.
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private readonly IMemberService _memberService;
        private readonly IApplicationRepo _applicationRepo;
        private readonly ICharacterService _characterService;
        private readonly IBossKillService _bossKillService;

        public ApplicationService(
            IMemberService memberService,
            IApplicationRepo applicationRepo,
            ICharacterService characterService,
            IBossKillService bossKillService)
        {
            _memberService = memberService;
            _applicationRepo = applicationRepo;
            _characterService = characterService;
            _bossKillService = bossKillService;
        }

        /// <summary>
        /// Adds a new application to the repository.
        /// </summary>
        public void AddApplication(Application application)
        {
            _applicationRepo.AddApplication(application);
        }

        /// <summary>
        /// Appends additional text to the application's note.
        /// </summary>
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

        /// <summary>
        /// Retrieves the note associated with a specific application.
        /// </summary>
        public string? GetNoteByApplicationId(int applicationId)
        {
            var app = _applicationRepo.GetApplicationById(applicationId);
            if (app == null) throw new Exception("Application not found.");
            return app.Note;
        }

        /// <summary>
        /// Approves an application and creates corresponding member and character if necessary.
        /// </summary>
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
                var insertedMember = _memberService.AddMember(newMember);

                var newCharacter = new Character
                {
                    CharacterName = application.CharacterName!,
                    Class = application.Class,
                    Role = application.Role,
                    MemberId = insertedMember.MemberId, // Correct after save
                    RealmName = application.ServerName
                };

                _characterService.AddCharacter(newCharacter);

                _bossKillService.TransferFromApplication(application, newCharacter.Id);
            }

            _applicationRepo.UpdateApplication(application);
        }

        /// <summary>
        /// Retrieves all applications that are pending approval.
        /// </summary>
        public List<Application> GetPendingApplications()
        {
            return _applicationRepo.GetApplications().Where(a => !a.Approved).ToList();
        }

        /// <summary>
        /// Submits a new application with associated boss kills.
        /// </summary>
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

        /// <summary>
        /// Retrieves all applications.
        /// </summary>
        public List<Application> GetAllApplications()
        {
            return _applicationRepo.GetApplications();
        }

        /// <summary>
        /// Retrieves a specific application by ID.
        /// </summary>
        public Application? GetApplicationById(int id)
        {
            return _applicationRepo.GetApplicationById(id);
        }

        /// <summary>
        /// Deletes an application by ID.
        /// </summary>
        public bool DeleteApplication(int id)
        {
            return _applicationRepo.DeleteApplication(id);
        }
    }
}
