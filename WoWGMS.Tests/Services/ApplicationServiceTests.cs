using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Service;

namespace WoWGMS.Tests.Services
{
    public class ApplicationServiceTests
    {
        // Mock dependencies
        private readonly Mock<IApplicationRepo> _mockApplicationrepo;
        private readonly Mock<IMemberService> _mockMemberService;
        private readonly Mock<ICharacterService> _mockCharacterService;
        private readonly Mock<IBossKillService> _mockBossKillService;

        // Service under test
        private readonly ApplicationService _service;

        public ApplicationServiceTests()
        {
            // Initialize mocks and service
            _mockApplicationrepo = new Mock<IApplicationRepo>();
            _mockMemberService = new Mock<IMemberService>();
            _mockCharacterService = new Mock<ICharacterService>();
            _mockBossKillService = new Mock<IBossKillService>();

            _service = new ApplicationService(
                _mockMemberService.Object,
                _mockApplicationrepo.Object,
                _mockCharacterService.Object,
                _mockBossKillService.Object
            );
        }

        [Fact]
        public void AppendToNote_AppendsTextWithoutAuthor_WhenNoteAlreadyExists()
        {
            // Arrange
            var mockRepo = new Mock<IApplicationRepo>();
            var app = new Application { ApplicationId = 1, Note = "Initial note" };
            mockRepo.Setup(r => r.GetApplicationById(1)).Returns(app);
            var service = new ApplicationService(null, mockRepo.Object, null, null);

            // Act
            service.AppendToNote(1, "Additional note");

            // Assert
            Assert.Equal("Initial note | Additional note", app.Note);
            mockRepo.Verify(r => r.UpdateApplication(app), Times.Once);
        }

        [Fact]
        public void AppendToNote_AppendsTextWithAuthor_WhenNoteAlreadyExists()
        {
            // Arrange
            var mockRepo = new Mock<IApplicationRepo>();
            var app = new Application { ApplicationId = 2, Note = "Initial note" };
            mockRepo.Setup(r => r.GetApplicationById(2)).Returns(app);
            var service = new ApplicationService(null, mockRepo.Object, null, null);

            // Act
            service.AppendToNote(2, "More info", "Admin");

            // Assert
            Assert.Equal("Initial note | [Admin] More info", app.Note);
            mockRepo.Verify(r => r.UpdateApplication(app), Times.Once);
        }

        [Fact]
        public void AppendToNote_SetsNote_WhenNoteIsInitiallyNull()
        {
            // Arrange
            var mockRepo = new Mock<IApplicationRepo>();
            var app = new Application { ApplicationId = 3, Note = null };
            mockRepo.Setup(r => r.GetApplicationById(3)).Returns(app);
            var service = new ApplicationService(null, mockRepo.Object, null, null);

            // Act
            service.AppendToNote(3, "First note");

            // Assert
            Assert.Equal("First note", app.Note);
            mockRepo.Verify(r => r.UpdateApplication(app), Times.Once);
        }

        [Fact]
        public void AppendToNote_ThrowsException_IfApplicationNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IApplicationRepo>();
            mockRepo.Setup(r => r.GetApplicationById(99)).Returns((Application)null);
            var service = new ApplicationService(null, mockRepo.Object, null, null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => service.AppendToNote(99, "Test"));
            Assert.Equal("Application not found.", ex.Message);
        }

        [Fact]
        public void AppendToNote_DoesNothing_WhenNoteIsWhitespace()
        {
            // Arrange
            var mockRepo = new Mock<IApplicationRepo>();
            var app = new Application { ApplicationId = 4, Note = "Something" };
            mockRepo.Setup(r => r.GetApplicationById(4)).Returns(app);
            var service = new ApplicationService(null, mockRepo.Object, null, null);

            // Act
            service.AppendToNote(4, "   "); // whitespace only

            // Assert
            Assert.Equal("Something", app.Note);
            mockRepo.Verify(r => r.UpdateApplication(It.IsAny<Application>()), Times.Never);
        }

        [Fact]
        public void ApproveApplication_ShouldAddNewMemberAndCharacter_WhenApplicationIsApproved()
        {
            // Arrange
            var application = new Application
            {
                ApplicationId = 1,
                Approved = false,
                DiscordName = "TestUser",
                Password = "TestPassword",
                CharacterName = "TestCharacter",
                Class = Class.Warrior,
                Role = Role.Tank,
                ServerName = ServerName.Aegwynn
            };

            // Mock no existing members (to allow new creation)
            _mockMemberService.Setup(m => m.GetMembers())
                .Returns(new List<Member>());

            // Mock AddMember to return a Member with a valid MemberId
            _mockMemberService.Setup(m => m.AddMember(It.IsAny<Member>()))
                .Returns((Member m) =>
                {
                    m.MemberId = 123;
                    return m;
                });

            // Verify UpdateApplication and AddCharacter calls
            _mockApplicationrepo.Setup(a => a.UpdateApplication(It.IsAny<Application>())).Verifiable();
            _mockCharacterService.Setup(c => c.AddCharacter(It.IsAny<Character>())).Verifiable();
            _mockBossKillService.Setup(b => b.TransferFromApplication(It.IsAny<Application>(), It.IsAny<int>())).Verifiable();

            // Act
            _service.ApproveApplication(application);

            // Assert
            _mockApplicationrepo.Verify(a => a.UpdateApplication(application), Times.Once);
            _mockMemberService.Verify(m => m.AddMember(It.Is<Member>(m =>
                m.Name == application.DiscordName &&
                m.Password == application.Password &&
                m.Rank == Rank.Trialist)), Times.Once);
            _mockCharacterService.Verify(c => c.AddCharacter(It.Is<Character>(c =>
                c.CharacterName == application.CharacterName &&
                c.Class == application.Class &&
                c.Role == application.Role &&
                c.RealmName == application.ServerName)), Times.Once);
        }

        [Fact]
        public void ApproveApplication_ShouldNotAddMember_WhenAlreadyExists()
        {
            // Arrange
            var application = new Application
            {
                ApplicationId = 1,
                Approved = false,
                DiscordName = "TestUser",
                Password = "TestPassword",
                CharacterName = "TestCharacter",
                Class = Class.Warrior,
                Role = Role.Tank,
                ServerName = ServerName.Aegwynn
            };

            // Mock existing member with the same DiscordName
            var existingMembers = new List<Member>
            {
                new Member { Name = "TestUser", Password = "TestPassword", Rank = Rank.Trialist }
            };
            _mockMemberService.Setup(m => m.GetMembers()).Returns(existingMembers);
            _mockApplicationrepo.Setup(a => a.UpdateApplication(It.IsAny<Application>())).Verifiable();

            // Act
            _service.ApproveApplication(application);

            // Assert
            _mockApplicationrepo.Verify(a => a.UpdateApplication(application), Times.Once);
            _mockMemberService.Verify(m => m.AddMember(It.IsAny<Member>()), Times.Never);
            _mockCharacterService.Verify(c => c.AddCharacter(It.IsAny<Character>()), Times.Never);
        }

        [Fact]
        public void GetPendingApplications_ShouldReturnOnlyUnapprovedApplications()
        {
            // Arrange
            var applications = new List<Application>
            {
                new Application { ApplicationId = 1, Approved = false },
                new Application { ApplicationId = 2, Approved = true },
                new Application { ApplicationId = 3, Approved = false }
            };
            _mockApplicationrepo.Setup(a => a.GetApplications()).Returns(applications);

            // Act
            var result = _service.GetPendingApplications();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.False(a.Approved));
        }

        [Fact]
        public void SubmitApplication_ShouldSetSubmissionDateAndAddApplication()
        {
            // Arrange
            var application = new Application
            {
                ApplicationId = 1,
                CharacterName = "TestCharacter",
                DiscordName = "TestUser",
                Password = "TestPassword",
                Class = Class.Warrior,
                Role = Role.Tank,
                ServerName = ServerName.Aegwynn
            };

            var bossKills = new Dictionary<string, int>
            {
                { "boss1", 2 },
                { "boss2", 1 }
            };
            _mockApplicationrepo.Setup(a => a.AddApplication(It.IsAny<Application>())).Verifiable();

            // Act
            _service.SubmitApplication(application, bossKills);

            // Assert
            _mockApplicationrepo.Verify(
                a => a.AddApplication(It.Is<Application>(a => a.SubmissionDate != default)),
                Times.Once
            );
        }

        [Fact]
        public void GetAllApplications_ShouldReturnAllApplications()
        {
            // Arrange
            var applications = new List<Application>
            {
                new Application { ApplicationId = 1 },
                new Application { ApplicationId = 2 }
            };
            _mockApplicationrepo.Setup(a => a.GetApplications()).Returns(applications);

            // Act
            var result = _service.GetAllApplications();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.ApplicationId == 1);
            Assert.Contains(result, a => a.ApplicationId == 2);
        }

        [Fact]
        public void AddApplication_ShouldCallRepoAddApplication()
        {
            // Arrange
            var application = new Application
            {
                ApplicationId = 1,
                CharacterName = "TestCharacter",
                DiscordName = "TestUser",
                Password = "TestPassword",
                Class = Class.Warrior,
                Role = Role.Tank,
                ServerName = ServerName.Aegwynn
            };
            _mockApplicationrepo.Setup(a => a.AddApplication(It.IsAny<Application>())).Verifiable();

            // Act
            _service.AddApplication(application);

            // Assert
            _mockApplicationrepo.Verify(a => a.AddApplication(application), Times.Once);
        }

        [Fact]
        public void ApproveApplication_ShouldSetSubmissionDate_WhenNotApproved()
        {
            // Arrange
            var application = new Application
            {
                ApplicationId = 1,
                Approved = false,
                DiscordName = "TestUser",
                Password = "TestPassword",
                CharacterName = "TestCharacter",
                Class = Class.Warrior,
                Role = Role.Tank,
                ServerName = ServerName.Aegwynn
            };

            _mockMemberService.Setup(m => m.GetMembers())
                .Returns(new List<Member>());

            _mockApplicationrepo.Setup(a => a.UpdateApplication(It.IsAny<Application>())).Verifiable();

            _mockMemberService.Setup(m => m.AddMember(It.IsAny<Member>()))
                .Returns((Member m) =>
                {
                    m.MemberId = 123;
                    return m;
                });

            _mockCharacterService.Setup(c => c.AddCharacter(It.IsAny<Character>())).Verifiable();
            _mockBossKillService.Setup(b => b.TransferFromApplication(It.IsAny<Application>(), It.IsAny<int>())).Verifiable();

            // Act
            _service.ApproveApplication(application);

            // Assert
            _mockApplicationrepo.Verify(a => a.UpdateApplication(It.Is<Application>(a => a.SubmissionDate != default)), Times.Once);
        }
    }
}
