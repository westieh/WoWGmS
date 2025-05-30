using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;
using WowGMSBackend.Registry;

namespace WoWGMS.Tests.Services
{
    public class RosterServiceTests
    {
        private readonly Mock<IRosterRepository> _mockRepo;
        private readonly Mock<ICharacterService> _characterService;
        private readonly RosterService _service;

        public RosterServiceTests()
        {
            _mockRepo = new Mock<IRosterRepository>();
            _characterService = new Mock<ICharacterService>();
            _service = new RosterService(_mockRepo.Object, _characterService.Object);
        }

        [Fact]
        public void AddCharacterToRoster_ShouldAdd_WhenCharacterIsUnique()
        {
            var character = new Character("TestChar", Class.Mage, Role.Healer, ServerName.Aegwynn) { MemberId = 1 };
            var roster = new BossRoster { RosterId = 1, Participants = new List<Character>() };
            _mockRepo.Setup(r => r.GetById(1)).Returns(roster);

            _service.AddCharacterToRoster(1, character);

            Assert.Single(roster.Participants);
            Assert.Equal("TestChar", roster.Participants[0].CharacterName);
            _mockRepo.Verify(r => r.Update(roster), Times.Once);
        }

        [Fact]
        public void RemoveCharacterFromRoster_ShouldRemove_WhenCharacterExists()
        {
            var charToRemove = new Character("CharB", Class.Rogue, Role.MeleeDPS, ServerName.TwistingNether);
            var roster = new BossRoster
            {
                RosterId = 2,
                Participants = new List<Character> { charToRemove }
            };

            _mockRepo.Setup(r => r.GetById(2)).Returns(roster);

            _service.RemoveCharacterFromRoster(2, charToRemove.Id);

            Assert.Empty(roster.Participants);
            _mockRepo.Verify(r => r.Update(roster), Times.Once);
        }

        [Fact]
        public void CheckRosterBalance_ShouldReturnTrue_WhenAllRolesPresent()
        {
            var roster = new BossRoster
            {
                Participants = new List<Character>
                {
                    new Character("T1", Class.Warrior, Role.Tank, ServerName.Aegwynn),
                    new Character("H1", Class.Mage, Role.Healer, ServerName.Aegwynn),
                    new Character("M1", Class.Rogue, Role.MeleeDPS, ServerName.Aegwynn),
                    new Character("R1", Class.Hunter, Role.RangedDPS, ServerName.Aegwynn)
                }
            };

            Assert.True(_service.CheckRosterBalance(roster));
        }

        [Fact]
        public void CheckRosterBalance_ShouldReturnFalse_WhenOneRoleIsMissing()
        {
            var roster = new BossRoster
            {
                Participants = new List<Character>
                {
                    new Character("T", Class.Warrior, Role.Tank, ServerName.Aegwynn),
                    new Character("H", Class.Paladin, Role.Healer, ServerName.Aegwynn),
                    new Character("M", Class.Rogue, Role.MeleeDPS, ServerName.Aegwynn)
                }
            };

            var result = _service.CheckRosterBalance(roster);
            Assert.False(result);
        }

        [Fact]
        public void ProcessRoster_ShouldIncrementBossKills_AndMarkAsProcessed()
        {
            var character = new Character("Char", Class.Warrior, Role.Tank, ServerName.Aegwynn);
            var roster = new BossRoster
            {
                RosterId = 10,
                RaidSlug = "liberation-of-undermine",
                BossDisplayName = "Vexie and the Geargrinders",
                InstanceTime = DateTime.UtcNow.AddHours(-1),
                IsProcessed = false,
                Participants = new List<Character> { character }
            };

            _mockRepo.Setup(r => r.GetById(10)).Returns(roster);

            _service.ProcessRoster(10);

            Assert.True(roster.IsProcessed);

            var expectedSlug = RaidRegistry
                .GetBossByDisplayName("liberation-of-undermine", "Vexie and the Geargrinders")?.Slug;

            Assert.NotNull(expectedSlug);
            var kill = character.BossKills.FirstOrDefault(bk => bk.BossSlug == expectedSlug);
            Assert.NotNull(kill);

            _mockRepo.Verify(r => r.Update(roster), Times.Once);
        }

        [Fact]
        public void GetUnprocessedRostersBefore_ShouldReturnFilteredAndOrderedList()
        {
            var rosters = new List<BossRoster>
            {
                new BossRoster { RosterId = 1, IsProcessed = false, InstanceTime = DateTime.UtcNow.AddHours(-3) },
                new BossRoster { RosterId = 2, IsProcessed = true, InstanceTime = DateTime.UtcNow.AddHours(-2) },
                new BossRoster { RosterId = 3, IsProcessed = false, InstanceTime = DateTime.UtcNow.AddHours(2) }
            };
            _mockRepo.Setup(r => r.GetAll()).Returns(rosters);

            var result = _service.GetUnprocessedRostersBefore(DateTime.UtcNow).ToList();

            Assert.Single(result);
            Assert.Equal(1, result[0].RosterId);
        }

        [Fact]
        public void GetEligibleCharacters_ShouldReturnOnlyNonParticipants()
        {
            var existingChar = new Character { Id = 1 };
            var newChar = new Character { Id = 2 };
            var roster = new BossRoster { Participants = new List<Character> { existingChar } };
            _characterService.Setup(s => s.GetCharacters()).Returns(new List<Character> { existingChar, newChar });
            var service = new RosterService(_mockRepo.Object, _characterService.Object);
            var result = service.GetEligibleCharacters(roster);
            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
        }

        [Fact]
        public void GetCharacterById_ShouldReturnCorrectCharacter()
        {
            var character = new Character { Id = 42 };
            _characterService.Setup(s => s.GetCharacter(42)).Returns(character);
            var service = new RosterService(_mockRepo.Object, _characterService.Object);
            var result = service.GetCharacterById(42);
            Assert.Equal(42, result?.Id);
        }

        [Fact]
        public void Update_ShouldCallRepositoryAndReturnUpdatedRoster()
        {
            var updated = new BossRoster { RosterId = 3 };
            _mockRepo.Setup(r => r.Update(updated)).Returns(updated);
            var result = _service.Update(updated);
            Assert.Equal(3, result?.RosterId);
        }

        [Fact]
        public void Delete_ShouldInvokeRepositoryDelete()
        {
            _service.Delete(5);
            _mockRepo.Verify(r => r.Delete(5), Times.Once);
        }

        [Fact]
        public void GetRosterById_ShouldReturnExpectedRoster()
        {
            var expected = new BossRoster { RosterId = 6 };
            _mockRepo.Setup(r => r.GetById(6)).Returns(expected);
            var result = _service.GetRosterById(6);
            Assert.Equal(6, result?.RosterId);
        }

        [Fact]
        public void GetRostersWithBosses_ShouldFilterByBossName()
        {
            var rosters = new List<BossRoster>
            {
                new BossRoster { BossDisplayName = "BossA" },
                new BossRoster { BossDisplayName = null }
            };
            _mockRepo.Setup(r => r.GetAll()).Returns(rosters);
            var result = _service.GetRostersWithBosses();
            Assert.Single(result);
            Assert.Equal("BossA", result[0].BossDisplayName);
        }

        [Fact]
        public void GetUpcomingRosters_ShouldReturnFutureSortedRosters()
        {
            var now = DateTime.Now;
            var rosters = new List<BossRoster>
            {
                new BossRoster { InstanceTime = now.AddHours(1) },
                new BossRoster { InstanceTime = now.AddHours(2) }
            };
            _mockRepo.Setup(r => r.GetAll()).Returns(rosters);
            var result = _service.GetUpcomingRosters();
            Assert.Equal(2, result.Count);
            Assert.True(result[0].InstanceTime <= result[1].InstanceTime);
        }

        [Fact]
        public void UpdateRosterTime_ShouldModifyInstanceTime()
        {
            var roster = new BossRoster { InstanceTime = DateTime.Now };
            _mockRepo.Setup(r => r.GetById(1)).Returns(roster);
            var newTime = DateTime.Now.AddDays(1);
            _service.UpdateRosterTime(1, newTime);
            Assert.Equal(newTime, roster.InstanceTime);
            _mockRepo.Verify(r => r.Update(roster), Times.Once);
        }

        [Fact]
        public void IsRosterAtCapacity_ShouldReturnTrue_IfParticipantsReachLimit()
        {
            var fullRoster = new BossRoster { Participants = Enumerable.Repeat(new Character(), 20).ToList() };
            _mockRepo.Setup(r => r.GetById(1)).Returns(fullRoster);
            var result = _service.IsRosterAtCapacity(1);
            Assert.True(result);
        }
    }
}