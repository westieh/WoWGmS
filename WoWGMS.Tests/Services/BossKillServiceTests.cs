using System.Collections.Generic;
using System.Linq;
using Moq;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Service;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
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
            var characterId = 1;
            var bossKills = new List<BossKill>
            {
                new BossKill { BossSlug = "boss1", CharacterId = characterId },
                new BossKill { BossSlug = "boss1", CharacterId = characterId },
                new BossKill { BossSlug = "boss2", CharacterId = characterId }
            };
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(characterId)).Returns(bossKills);

            var result = _service.GetMostKilledBossForCharacter(characterId);

            Assert.NotNull(result);
            Assert.Equal("boss1", result.BossSlug);
        }

        [Fact]
        public void SetBossKillsForCharacter_ClearsAndAddsKills()
        {
            int characterId = 2;
            var kills = new List<BossKill>
            {
                new BossKill { BossSlug = "boss1" },
                new BossKill { BossSlug = "boss2" }
            };

            _service.SetBossKillsForCharacter(characterId, kills);

            _mockBossKillRepo.Verify(r => r.DeleteBossKillsForCharacter(characterId), Times.Once);
            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(k => k.BossSlug == "boss1" && k.CharacterId == characterId)), Times.Once);
            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(k => k.BossSlug == "boss2" && k.CharacterId == characterId)), Times.Once);
        }

        [Fact]
        public void IncrementBossKill_AddsOrIncrements()
        {
            int characterId = 3;
            string bossSlug = "boss3";
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(characterId)).Returns(new List<BossKill>());

            _service.IncrementBossKill(characterId, bossSlug);

            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(b => b.BossSlug == bossSlug && b.CharacterId == characterId)), Times.Once);
        }

        [Fact]
        public void TransferFromApplication_TransfersCorrectly()
        {
            var app = new Application
            {
                BossKills = new List<BossKill> { new BossKill { BossSlug = "boss1" } }
            };
            int characterId = 4;

            _service.TransferFromApplication(app, characterId);

            _mockBossKillRepo.Verify(r => r.AddBossKill(It.Is<BossKill>(b => b.BossSlug == "boss1" && b.CharacterId == characterId)), Times.Once);
        }

        [Fact]
        public void GetBossKillCountsForRoster_ReturnsCorrectCounts()
        {
            var roster = new BossRoster
            {
                Participants = new List<Character> {
                    new Character { Id = 10 },
                    new Character { Id = 20 }
                }
            };
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(10))
                            .Returns(new List<BossKill>
                            {
                                    new BossKill { KillCount = 1 },
                                    new BossKill { KillCount = 1 }
                            });

            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(20))
                            .Returns(new List<BossKill>
                            {
                                   new BossKill { KillCount = 1 }
                            });
            var result = _service.GetBossKillCountsForRoster(roster);

            Assert.Equal(2, result[10]);
            Assert.Equal(1, result[20]);
        }

        [Fact]
        public void GetBossKillsForCharacter_ReturnsExpectedKills()
        {
            int characterId = 5;
            var expectedKills = new List<BossKill> { new BossKill(), new BossKill() };
            _mockBossKillRepo.Setup(r => r.GetBossKillsByCharacterId(characterId)).Returns(expectedKills);

            var result = _service.GetBossKillsForCharacter(characterId);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllBosses_ReturnsFromRegistry()
        {
            var result = _service.GetAllBosses();
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }

        [Fact]
        public void SetOrUpdateSingleBossKill_UpdatesCorrectly()
        {
            int characterId = 6;
            string bossSlug = "boss4";
            int newCount = 3;

            var character = new Character
            {
                Id = characterId,
                BossKills = new List<BossKill>()
            };

            _mockCharacterService.Setup(s => s.GetCharacter(characterId)).Returns(character);

            _service.SetOrUpdateSingleBossKill(characterId, bossSlug, newCount);

            _mockCharacterService.Verify(s =>
                s.UpdateCharacter(characterId, It.Is<Character>(c =>
                    c.BossKills.Any(k => k.BossSlug == bossSlug && k.KillCount == newCount)
                )),
                Times.Once
            );
        }
    }
}