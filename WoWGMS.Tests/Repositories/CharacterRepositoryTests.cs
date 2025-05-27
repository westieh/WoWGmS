using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

public class CharacterRepositoryTests
{
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
        var character = CreateTestCharacter(member.MemberId);
        var result = repo.AddCharacter(character);

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
        var fetched = repo.GetCharacter(character.Id);

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
        var updated = CreateTestCharacter(member.MemberId, "CharA");
        updated.Role = Role.Healer;
        var result = repo.UpdateCharacter(character.Id, updated);

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
        var removed = repo.DeleteCharacter(character.Id);

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
        var result = repo.GetCharactersByMemberId(member.MemberId);

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
        var all = repo.GetAllCharacters();

        Assert.Equal(2, all.Count);
    }
}
