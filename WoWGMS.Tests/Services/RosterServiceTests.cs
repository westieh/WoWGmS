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
        private readonly RosterService _service;
        private readonly Mock<ICharacterService> _characterService;

        public RosterServiceTests()
        {
            _mockRepo = new Mock<IRosterRepository>();
            _service = new RosterService(_mockRepo.Object);
            _characterService = new Mock<ICharacterService>();
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

            _service.RemoveCharacterFromRoster(2, "CharB", "TwistingNether");

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
            Console.WriteLine("Expected Slug: " + expectedSlug);
            Console.WriteLine("Character BossKills Keys: " + string.Join(", ", character.BossKills.Keys));
            Assert.NotNull(expectedSlug);
            Assert.True(character.BossKills.ContainsKey(expectedSlug));
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
    }
}
