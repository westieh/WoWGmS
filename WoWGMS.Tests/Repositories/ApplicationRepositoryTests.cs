using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

public class ApplicationRepositoryTests
{
    private WowDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WowDbContext(options);
    }

    private Application CreateTestApplication(string charName = "CharX", string discord = "User#1234")
    {
        return new Application
        {
            CharacterName = charName,
            DiscordName = discord,
            Password = "testpass",
            Class = Class.Warrior,
            Role = Role.Tank,
            ServerName = ServerName.Aegwynn,
            SubmissionDate = DateTime.UtcNow,
            Approved = false,
            Note = "Initial",
            ProcessedBy = null
        };
    }

    [Fact]
    public void AddApplication_ShouldInsert()
    {
        using var context = GetInMemoryContext();
        var repo = new ApplicationRepo(context);

        var app = CreateTestApplication();
        repo.AddApplication(app);

        Assert.Single(context.Applications);
        Assert.Equal("CharX", context.Applications.First().CharacterName);
    }

    [Fact]
    public void GetApplicationById_ShouldReturnMatch()
    {
        using var context = GetInMemoryContext();
        var repo = new ApplicationRepo(context);

        var app = CreateTestApplication("MatchMe", "Test#0001");
        context.Applications.Add(app);
        context.SaveChanges();

        var found = repo.GetApplicationById(app.ApplicationId);
        Assert.NotNull(found);
        Assert.Equal("MatchMe", found.CharacterName);
    }

    [Fact]
    public void GetApplications_ShouldReturnAll()
    {
        using var context = GetInMemoryContext();
        var repo = new ApplicationRepo(context);

        context.Applications.AddRange(
            CreateTestApplication("Char1", "One#1111"),
            CreateTestApplication("Char2", "Two#2222")
        );
        context.SaveChanges();

        var result = repo.GetApplications();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void UpdateApplication_ShouldModifyValues()
    {
        using var context = GetInMemoryContext();
        var repo = new ApplicationRepo(context);

        var app = CreateTestApplication("CharOld", "Old#0000");
        context.Applications.Add(app);
        context.SaveChanges();

        var updated = CreateTestApplication("CharNew", "New#0000");
        updated.ApplicationId = app.ApplicationId;
        updated.Approved = true;
        updated.Note = "Reviewed";

        var result = repo.UpdateApplication(updated);
        Assert.True(result);

        var reloaded = context.Applications.First(a => a.ApplicationId == app.ApplicationId);
        Assert.Equal("CharNew", reloaded.CharacterName);
        Assert.True(reloaded.Approved);
        Assert.Equal("Reviewed", reloaded.Note);
    }

    [Fact]
    public void DeleteApplication_ShouldRemoveFromDb()
    {
        using var context = GetInMemoryContext();
        var repo = new ApplicationRepo(context);

        var app = CreateTestApplication("CharDel", "Del#1234");
        context.Applications.Add(app);
        context.SaveChanges();

        var deleted = repo.DeleteApplication(app.ApplicationId);
        Assert.True(deleted);
        Assert.Empty(context.Applications);
    }
}
