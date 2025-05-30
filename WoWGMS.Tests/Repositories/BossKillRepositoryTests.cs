using Xunit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WowGMSBackend.DBContext;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

public class BossKillRepositoryTests
{
    private WowDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WowDbContext(options);
    }

    [Fact]
    public void AddBossKill_ShouldInsertIntoDatabase()
    {
        using var context = GetInMemoryContext();
        var repo = new BossKillRepo(context);

        var kill = new BossKill { CharacterId = 1, BossSlug = "slug-a", KillCount = 2 };
        repo.AddBossKill(kill);

        Assert.Single(context.BossKills);
        Assert.Equal("slug-a", context.BossKills.First().BossSlug);
    }

    [Fact]
    public void DeleteBossKillsForCharacter_ShouldRemoveAllKills()
    {
        using var context = GetInMemoryContext();
        var repo = new BossKillRepo(context);

        context.BossKills.AddRange(
            new BossKill { CharacterId = 1, BossSlug = "a" },
            new BossKill { CharacterId = 1, BossSlug = "b" },
            new BossKill { CharacterId = 2, BossSlug = "x" }
        );
        context.SaveChanges();

        repo.DeleteBossKillsForCharacter(1);

        Assert.Single(context.BossKills);
        Assert.Equal(2, context.BossKills.First().CharacterId);
    }

    [Fact]
    public void GetBossKillsByCharacterId_ShouldReturnOnlyMatches()
    {
        using var context = GetInMemoryContext();
        var repo = new BossKillRepo(context);

        context.BossKills.AddRange(
            new BossKill { CharacterId = 1, BossSlug = "a" },
            new BossKill { CharacterId = 1, BossSlug = "b" },
            new BossKill { CharacterId = 2, BossSlug = "x" }
        );
        context.SaveChanges();

        var result = repo.GetBossKillsByCharacterId(1);

        Assert.Equal(2, result.Count);
    }

}
