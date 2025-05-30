using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;
using WowGMSBackend.DBContext;

public class MemberRepositoryTests
{
    // Creates an in-memory database context for testing
    private WowDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WowDbContext(options);
    }

    private Member CreateTestMember(string name = "TestUser", string password = "1234", Rank rank = Rank.Trialist)
    {
        return new Member
        {
            Name = name,
            Password = password,
            Rank = rank
        };
    }

    [Fact]
    public void AddMember_ShouldInsertMember()
    {
        using var context = GetInMemoryContext();
        var repo = new MemberRepo(context);

        // Act
        var member = CreateTestMember();
        var result = repo.AddMember(member);

        // Assert
        Assert.Equal(member.Name, result.Name);
        Assert.Single(context.Members);
    }

    [Fact]
    public void GetMember_ShouldReturnCorrectMember()
    {
        using var context = GetInMemoryContext();
        var repo = new MemberRepo(context);

        // Arrange
        var member = CreateTestMember("Alice");
        context.Members.Add(member);
        context.SaveChanges();

        // Act
        var fetched = repo.GetMember(member.MemberId);

        // Assert
        Assert.NotNull(fetched);
        Assert.Equal("Alice", fetched.Name);
    }

    [Fact]
    public void UpdateMember_ShouldModifyNameAndRank()
    {
        using var context = GetInMemoryContext();
        var repo = new MemberRepo(context);

        // Arrange
        var original = CreateTestMember("OldName", "pass", Rank.Trialist);
        context.Members.Add(original);
        context.SaveChanges();

        // Act
        var updated = CreateTestMember("NewName", "pass", Rank.Officer);
        var result = repo.UpdateMember(original.MemberId, updated);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewName", result.Name);
        Assert.Equal(Rank.Officer, result.Rank);
    }

    [Fact]
    public void DeleteMember_ShouldRemoveMember()
    {
        using var context = GetInMemoryContext();
        var repo = new MemberRepo(context);

        // Arrange
        var member = CreateTestMember("DeleteMe");
        context.Members.Add(member);
        context.SaveChanges();

        // Act
        var removed = repo.DeleteMember(member.MemberId);

        // Assert
        Assert.Equal("DeleteMe", removed.Name);
        Assert.Empty(context.Members);
    }

    [Fact]
    public void GetMembers_ShouldReturnAllMembers()
    {
        using var context = GetInMemoryContext();
        var repo = new MemberRepo(context);

        // Arrange
        context.Members.AddRange(
            CreateTestMember("One"),
            CreateTestMember("Two")
        );
        context.SaveChanges();

        // Act
        var members = repo.GetMembers();

        // Assert
        Assert.Equal(2, members.Count);
    }
}
