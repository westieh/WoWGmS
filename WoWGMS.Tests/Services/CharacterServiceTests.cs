using Xunit;
using Moq;
using System.Collections.Generic;
using WowGMSBackend.Repository;
using WowGMSBackend.Service;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
namespace WoWGMS.Tests.Services
{
    public class CharacterServiceTests
    {
        private readonly Mock<ICharacterRepo> _mockRepo;
        private readonly CharacterService _service;

        public CharacterServiceTests()
        {
            _mockRepo = new Mock<ICharacterRepo>();
            _service = new CharacterService(_mockRepo.Object);
        }

        [Fact]
        public void AddCharacter_ShouldCallRepoAndReturnCharacter()
        {
            var character = new Character("TestCharacter", Class.Warrior, Role.Tank, ServerName.Aegwynn)
            {
                Id = 1,
                MemberId = 42
            };

            _mockRepo.Setup(r => r.AddCharacter(character)).Returns(character);

            var result = _service.AddCharacter(character);

            _mockRepo.Verify(r => r.AddCharacter(character), Times.Once);
            Assert.Equal("TestCharacter", result.CharacterName);
        }

        [Fact]
        public void GetCharacter_ShouldCallRepoAndReturnCharacter()
        {
            var character = new Character("TestCharacter", Class.Warrior, Role.Tank, ServerName.Aegwynn)
            {
                Id = 1,
                MemberId = 42
            };

            _mockRepo.Setup(r => r.GetCharacter(1)).Returns(character);

            var result = _service.GetCharacter(1);

            _mockRepo.Verify(r => r.GetCharacter(1), Times.Once);
            Assert.Equal(character, result);
        }

        [Fact]
        public void GetCharacters_ShouldReturnAllCharacters()
        {
            var characters = new List<Character>
            {
                new Character("CharOne", Class.Warrior, Role.Tank, ServerName.Aegwynn) { Id = 1, MemberId = 42 },
                new Character("CharTwo", Class.Mage, Role.Healer, ServerName.TwistingNether) { Id = 2, MemberId = 99 }
            };

            _mockRepo.Setup(r => r.GetCharacters()).Returns(characters);

            var result = _service.GetCharacters();

            _mockRepo.Verify(r => r.GetCharacters(), Times.Once);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void UpdateCharacter_ShouldCallRepoAndReturnUpdatedCharacter()
        {
            var updated = new Character("UpdatedCharacter", Class.Paladin, Role.Healer, ServerName.Aegwynn)
            {
                Id = 1,
                MemberId = 42
            };

            _mockRepo.Setup(r => r.UpdateCharacter(1, updated)).Returns(updated);

            var result = _service.UpdateCharacter(1, updated);

            _mockRepo.Verify(r => r.UpdateCharacter(1, It.Is<Character>(c =>
                c.CharacterName == "UpdatedCharacter" &&
                c.Class == Class.Paladin &&
                c.Role == Role.Healer &&
                c.RealmName == ServerName.Aegwynn &&
                c.MemberId == 42
            )), Times.Once);

            Assert.Equal("UpdatedCharacter", result.CharacterName);
        }
        [Fact]
        public void DeleteCharacter_ShouldCallRepoAndReturnDeletedCharacter()
        {
            var character = new Character("ToDelete", Class.Rogue, Role.MeleeDPS, ServerName.Aegwynn)
            {
                Id = 3,
                MemberId = 51
            };

            _mockRepo.Setup(r => r.DeleteCharacter(3)).Returns(character);

            var result = _service.DeleteCharacter(3);

            _mockRepo.Verify(r => r.DeleteCharacter(3), Times.Once);
            Assert.Equal("ToDelete", result.CharacterName);
        }
        // Doesnt verify the call, tests for the result. 
        [Fact]
        public void IncrementBossKill_ShouldAddBossKill_WhenCharacterExists()
        {
            var character = new Character("TestChar", Class.Mage, Role.Healer, ServerName.Aegwynn)
            {
                Id = 1,
                MemberId = 42
            };

            _mockRepo.Setup(r => r.GetCharacter(1)).Returns(character);

            _service.IncrementBossKill(1, "onyxia");
            var onyKill = character.BossKills.FirstOrDefault(bk => bk.BossSlug == "onyxia");

            Assert.NotNull(onyKill);
            Assert.Equal(1, onyKill.KillCount);
        }
        

        [Fact]
        public void IncrementBossKill_ShouldDoNothing_WhenCharacterIsNull()
        {
            _mockRepo.Setup(r => r.GetCharacter(99)).Returns((Character)null);

            var ex = Record.Exception(() => _service.IncrementBossKill(99, "any-boss"));

            Assert.Null(ex); // should not throw
        }
        [Fact]
        public void GetCharactersByMemberId_ShouldReturnCharactersForGivenMemberId()
        {
            var characters = new List<Character>
            {
                new Character("CharOne", Class.Warrior, Role.Tank, ServerName.Aegwynn) { Id = 1, MemberId = 42 },
                new Character("CharTwo", Class.Mage, Role.Healer, ServerName.TwistingNether) { Id = 2, MemberId = 42 }
            };
            _mockRepo.Setup(r => r.GetCharactersByMemberId(42)).Returns(characters);
            var result = _service.GetCharactersByMemberId(42);
            _mockRepo.Verify(r => r.GetCharactersByMemberId(42), Times.Once);
            Assert.Equal(2, result.Count);
        }
    }
}

