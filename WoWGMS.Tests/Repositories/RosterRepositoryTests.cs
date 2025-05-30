using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

public class RosterRepositoryTests
{
    // Creates an in-memory database context for testing
    private WowDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WowDbContext(options);
    }

    [Fact]
    public void Add_ShouldInsertRoster()
    {
        using var context = GetInMemoryContext();
        var repo = new RosterRepository(context);

        // Act
        var roster = new BossRoster
        {
            RaidSlug = "liberation-of-undermine",
            BossDisplayName = "Vexie and the Geargrinders",
            BossSlug = "vexie-and-the-geargrinders",
            InstanceTime = DateTime.UtcNow
        };
        var result = repo.Add(roster);

        // Assert
        Assert.Equal(roster.BossDisplayName, result.BossDisplayName);
        Assert.Single(context.BossRosters);
    }

    [Fact]
    public void GetById_ShouldReturnCorrectRoster()
    {
        using var context = GetInMemoryContext();
        var repo = new RosterRepository(context);

        // Arrange
        var roster = new BossRoster
        {
            BossDisplayName = "Rashok",
            BossSlug = "rashok",
            RaidSlug = "aberrus",
            InstanceTime = DateTime.UtcNow
        };
        context.BossRosters.Add(roster);
        context.SaveChanges();

        // Act
        var result = repo.GetById(roster.RosterId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rashok", result.BossDisplayName);
    }

    [Fact]
    public void Update_ShouldChangeBossDisplayName()
    {
        using var context = GetInMemoryContext();
        var repo = new RosterRepository(context);

        // Arrange
        var roster = new BossRoster
        {
            BossDisplayName = "Old",
            BossSlug = "old-slug",
            RaidSlug = "aberrus",
            InstanceTime = DateTime.UtcNow
        };
        context.BossRosters.Add(roster);
        context.SaveChanges();

        // Act
        roster.BossDisplayName = "New";
        repo.Update(roster);

        // Assert
        var updated = context.BossRosters.Find(roster.RosterId);
        Assert.Equal("New", updated.BossDisplayName);
    }

    [Fact]
    public void Delete_ShouldRemoveRoster()
    {
        using var context = GetInMemoryContext();
        var repo = new RosterRepository(context);

        // Arrange
        var roster = new BossRoster
        {
            BossDisplayName = "Delete Me",
            BossSlug = "delete-me",
            RaidSlug = "aberrus",
            InstanceTime = DateTime.UtcNow
        };
        context.BossRosters.Add(roster);
        context.SaveChanges();

        // Act
        var removed = repo.Delete(roster.RosterId);

        // Assert
        Assert.Equal("Delete Me", removed.BossDisplayName);
        Assert.Empty(context.BossRosters);
    }

    [Fact]
    public void GetAll_ShouldReturnAllRosters()
    {
        using var context = GetInMemoryContext();
        var repo = new RosterRepository(context);

        // Arrange
        context.BossRosters.AddRange(
            new BossRoster { BossDisplayName = "One", BossSlug = "one", RaidSlug = "aberrus", InstanceTime = DateTime.UtcNow },
            new BossRoster { BossDisplayName = "Two", BossSlug = "two", RaidSlug = "aberrus", InstanceTime = DateTime.UtcNow }
        );
        context.SaveChanges();

        // Act
        var all = repo.GetAll().ToList();

        // Assert
        Assert.Equal(2, all.Count);
    }
}
