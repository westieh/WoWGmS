using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

namespace WoWGMS.Tests.Repositories
{
    public class CharacterRepositoryTests
    {
        // Creates an in-memory database context for testing
        private WowDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<WowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new WowDbContext(options);
        }

        private Member CreateTestMember(string name = "TestUser")
        {
            return new Member { Name = name, Password = "pass", Rank = Rank.Trialist };
        }

        private Character CreateTestCharacter(int memberId, string name = "CharA")
        {
            return new Character
            {
                CharacterName = name,
                Class = Class.Warrior,
                Role = Role.Tank,
                RealmName = ServerName.Aegwynn,
                MemberId = memberId
            };
        }

        [Fact]
        public void AddCharacter_ShouldInsertCharacter()
        {
            using var context = GetInMemoryContext();
            var member = CreateTestMember();
            context.Members.Add(member);
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var character = CreateTestCharacter(member.MemberId);
            var result = repo.AddCharacter(character);

            // Assert
            Assert.Equal("CharA", result.CharacterName);
            Assert.Single(context.Characters);
        }

        [Fact]
        public void GetCharacter_ShouldReturnCorrectCharacter()
        {
            using var context = GetInMemoryContext();
            var member = CreateTestMember();
            context.Members.Add(member);
            context.SaveChanges();

            var character = CreateTestCharacter(member.MemberId);
            context.Characters.Add(character);
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var fetched = repo.GetCharacter(character.Id);

            // Assert
            Assert.NotNull(fetched);
            Assert.Equal("CharA", fetched.CharacterName);
        }

        [Fact]
        public void UpdateCharacter_ShouldModifyFields()
        {
            using var context = GetInMemoryContext();
            var member = CreateTestMember();
            context.Members.Add(member);
            context.SaveChanges();

            var character = CreateTestCharacter(member.MemberId);
            context.Characters.Add(character);
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var updated = CreateTestCharacter(member.MemberId, "CharA");
            updated.Role = Role.Healer;
            var result = repo.UpdateCharacter(character.Id, updated);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Role.Healer, result.Role);
        }

        [Fact]
        public void DeleteCharacter_ShouldRemoveCharacter()
        {
            using var context = GetInMemoryContext();
            var member = CreateTestMember();
            context.Members.Add(member);
            context.SaveChanges();

            var character = CreateTestCharacter(member.MemberId);
            context.Characters.Add(character);
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var removed = repo.DeleteCharacter(character.Id);

            // Assert
            Assert.Equal("CharA", removed.CharacterName);
            Assert.Empty(context.Characters);
        }

        [Fact]
        public void GetCharactersByMemberId_ShouldReturnCorrectCharacters()
        {
            using var context = GetInMemoryContext();
            var member = CreateTestMember("MemberOne");
            context.Members.Add(member);
            context.SaveChanges();

            var character1 = CreateTestCharacter(member.MemberId, "Char1");
            var character2 = CreateTestCharacter(member.MemberId, "Char2");
            context.Characters.AddRange(character1, character2);
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var result = repo.GetCharactersByMemberId(member.MemberId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllCharacters_ShouldReturnAll()
        {
            using var context = GetInMemoryContext();
            var member = CreateTestMember();
            context.Members.Add(member);
            context.SaveChanges();

            context.Characters.AddRange(
                CreateTestCharacter(member.MemberId, "Char1"),
                CreateTestCharacter(member.MemberId, "Char2")
            );
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var all = repo.GetAllCharacters();

            // Assert
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public void AddBossKill_ShouldInsertKill()
        {
            using var context = GetInMemoryContext();
            var repo = new CharacterRepo(context);

            // Act
            var kill = new BossKill { BossSlug = "boss-slug", CharacterId = 1, KillCount = 3 };
            repo.AddBossKill(kill);

            // Assert
            Assert.Single(context.BossKills);
            Assert.Equal("boss-slug", context.BossKills.First().BossSlug);
        }

        [Fact]
        public void GetCharacters_ShouldIncludeRelatedData()
        {
            using var context = GetInMemoryContext();
            var member = CreateTestMember();
            var character = CreateTestCharacter(1);
            character.Member = member;
            character.BossKills.Add(new BossKill { BossSlug = "test-boss", KillCount = 1 });
            context.Characters.Add(character);
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var result = repo.GetCharacters();

            // Assert
            Assert.Single(result);
            Assert.Equal("test-boss", result[0].BossKills.First().BossSlug);
            Assert.Equal("TestUser", result[0].Member.Name);
        }

        [Fact]
        public void GetCharactersByRoster_ShouldReturnCorrectlyJoinedCharacters()
        {
            using var context = GetInMemoryContext();
            var character = new Character { CharacterName = "RosterChar" };

            var roster = new BossRoster
            {
                BossDisplayName = "Test Boss",
                BossSlug = "test-boss",
                RaidSlug = "test-raid",
                Participants = new List<Character> { character }
            };

            context.Characters.Add(character);
            context.BossRosters.Add(roster);
            context.SaveChanges();

            var repo = new CharacterRepo(context);

            // Act
            var result = repo.GetCharactersByRoster(roster.RosterId);

            // Assert
            Assert.Single(result);
            Assert.Equal("RosterChar", result[0].CharacterName);
        }
    }
}
