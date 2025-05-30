using System.Collections.Generic;
using System.Linq;
using Moq;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;
using Xunit;

namespace WoWgmsBackend.Tests.Services
{
    public class BossKillServiceTests
    {
        private readonly Mock<IBossKillRepo> _mockBossKillRepo;
        private readonly Mock<ICharacterService> _mockCharacterService;
        private readonly BossKillService _service;

        public BossKillServiceTests()
        {
            _mockBossKillRepo = new Mock<IBossKillRepo>();
            _mockCharacterService = new Mock<ICharacterService>();
            _service = new BossKillService(_mockBossKillRepo.Object, _mockCharacterService.Object);
        }

        [Fact]
        public void GetMostKilledBossForCharacter_ReturnsCorrectBoss()
        {
            // Arrange
            var characterId = 1;
            var bossKills = new List<BossKill>
            {
                new BossKill { BossSlug = "boss1", CharacterId = characterId },
                new BossKill { BossSlug = "boss1", CharacterId = characterId },
                new BossKill { BossSlug = "boss2", CharacterId = characterId }
            };
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(characterId)).Returns(bossKills);

            // Act
            var result = _service.GetMostKilledBossForCharacter(characterId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("boss1", result.BossSlug);
        }

        [Fact]
        public void SetBossKillsForCharacter_ClearsAndAddsKills()
        {
            // Arrange
            int characterId = 2;
            var kills = new List<BossKill>
            {
                new BossKill { BossSlug = "boss1" },
                new BossKill { BossSlug = "boss2" }
            };

            // Act
            _service.SetBossKillsForCharacter(characterId, kills);

            // Assert
            _mockBossKillRepo.Verify(r => r.DeleteBossKillsForCharacter(characterId), Times.Once);
            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(k => k.BossSlug == "boss1" && k.CharacterId == characterId)), Times.Once);
            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(k => k.BossSlug == "boss2" && k.CharacterId == characterId)), Times.Once);
        }

        [Fact]
        public void IncrementBossKill_AddsOrIncrements()
        {
            // Arrange
            int characterId = 3;
            string bossSlug = "boss3";
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(characterId)).Returns(new List<BossKill>());

            // Act
            _service.IncrementBossKill(characterId, bossSlug);

            // Assert
            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(b => b.BossSlug == bossSlug && b.CharacterId == characterId)), Times.Once);
        }

        [Fact]
        public void TransferFromApplication_TransfersCorrectly()
        {
            // Arrange
            var app = new Application
            {
                BossKills = new List<BossKill> { new BossKill { BossSlug = "boss1" } }
            };
            int characterId = 4;

            // Act
            _service.TransferFromApplication(app, characterId);

            // Assert
            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(b => b.BossSlug == "boss1" && b.CharacterId == characterId)), Times.Once);
        }

        [Fact]
        public void GetBossKillCountsForRoster_ReturnsCorrectCounts()
        {
            // Arrange
            var roster = new BossRoster
            {
                Participants = new List<Character> {
                    new Character { Id = 10 },
                    new Character { Id = 20 }
                }
            };
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(10)).Returns(new List<BossKill> { new BossKill { KillCount = 1 }, new BossKill { KillCount = 1 } });
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(20)).Returns(new List<BossKill> { new BossKill { KillCount = 1 } });

            // Act
            var result = _service.GetBossKillCountsForRoster(roster);

            // Assert
            Assert.Equal(2, result[10]);
            Assert.Equal(1, result[20]);
        }

        [Fact]
        public void GetBossKillsForCharacter_ReturnsExpectedKills()
        {
            // Arrange
            int characterId = 5;
            var expectedKills = new List<BossKill> { new BossKill(), new BossKill() };
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(characterId)).Returns(expectedKills);

            // Act
            var result = _service.GetBossKillsForCharacter(characterId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllBosses_ReturnsFromRegistry()
        {
            // Act
            var result = _service.GetAllBosses();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void SetOrUpdateSingleBossKill_UpdatesCorrectly()
        {
            // Arrange
            int characterId = 6;
            string bossSlug = "boss4";
            int newCount = 3;
            var character = new Character
            {
                Id = characterId,
                BossKills = new List<BossKill>()
            };
            _mockCharacterService.Setup(s => s.GetCharacter(characterId)).Returns(character);

            // Act
            _service.SetOrUpdateSingleBossKill(characterId, bossSlug, newCount);

            // Assert
            _mockCharacterService.Verify(s =>
                s.UpdateCharacter(characterId, It.Is<Character>(c =>
                    c.BossKills.Any(k => k.BossSlug == bossSlug && k.KillCount == newCount)
                )),
                Times.Once
            );
        }
    }
}
