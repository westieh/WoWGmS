using Xunit;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WowGMSBackend.Model;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Service;
using System.Linq;
using Moq.Protected;

namespace WoWGMS.Tests.Services
{
    public class BossKillCheckerServiceTests
    {
        private class TestableBossKillCheckerService : BossKillCheckerService
        {
            public TestableBossKillCheckerService(
                IHttpClientFactory httpClientFactory,
                IServiceScopeFactory scopeFactory,
                IConfiguration config)
                : base(httpClientFactory, scopeFactory, config) { }

            public new async Task CheckBossKills() => await base.CheckBossKills();
        }

        private IConfiguration BuildTestConfig() =>
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RaiderIO:ApiBase", "https://fake-raider.io" },
                { "RaiderIO:Region", "eu" },
                { "RaiderIO:Realm", "twisting-nether" },
                { "RaiderIO:Guild", "deathproof" },
                { "RaiderIO:Difficulty", "heroic" },
                { "RaiderIO:AccessKey", "mocked-key" }
            }).Build();

        [Fact]
        public async Task CheckBossKills_ShouldCallProcessRoster_WhenKillConfirmed()
        {
            var testRoster = new BossRoster
            {
                RosterId = 1,
                RaidSlug = "liberation-of-undermine",
                BossDisplayName = "Vexie and the Geargrinders",
                InstanceTime = DateTime.UtcNow.AddHours(-1),
                IsProcessed = false,
                Participants = new List<Character>()
            };

            var mockRosterService = new Mock<IRosterService>();
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>()))
                             .Returns(new List<BossRoster> { testRoster });

            var json = JsonSerializer.Serialize(new
            {
                kill = new { isSuccess = true, defeatedAt = DateTime.UtcNow.ToString("o") }
            });

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(json) });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            await service.CheckBossKills();

            mockRosterService.Verify(s => s.Update(It.Is<BossRoster>(r => r.RosterId == 1)), Times.Once);
        }

        [Fact]
        public async Task CheckBossKills_ShouldNotProcess_WhenApiReturns404()
        {
            var testRoster = new BossRoster
            {
                RosterId = 1,
                RaidSlug = "liberation-of-undermine",
                BossDisplayName = "Vexie and the Geargrinders",
                InstanceTime = DateTime.UtcNow.AddHours(-1),
                IsProcessed = false,
                Participants = new List<Character>()
            };

            var mockRosterService = new Mock<IRosterService>();
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>()))
                             .Returns(new List<BossRoster> { testRoster });

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            await service.CheckBossKills();

            mockRosterService.Verify(s => s.ProcessRoster(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CheckBossKills_ShouldNotProcess_WhenKillIsUnsuccessful()
        {
            var testRoster = new BossRoster { RosterId = 1, RaidSlug = "liberation-of-undermine", BossDisplayName = "Vexie and the Geargrinders", InstanceTime = DateTime.UtcNow.AddHours(-1), IsProcessed = false, Participants = new List<Character>() };

            var mockRosterService = new Mock<IRosterService>();
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>()))
                             .Returns(new List<BossRoster> { testRoster });

            var json = JsonSerializer.Serialize(new { kill = new { isSuccess = false, defeatedAt = DateTime.UtcNow.ToString("o") } });

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(json) });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            await service.CheckBossKills();

            mockRosterService.Verify(s => s.ProcessRoster(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CheckBossKills_ShouldNotProcess_WhenDefeatedAtIsInFuture()
        {
            var testRoster = new BossRoster { RosterId = 1, RaidSlug = "liberation-of-undermine", BossDisplayName = "Vexie and the Geargrinders", InstanceTime = DateTime.UtcNow.AddHours(-1), IsProcessed = false, Participants = new List<Character>() };

            var mockRosterService = new Mock<IRosterService>();
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>()))
                             .Returns(new List<BossRoster> { testRoster });

            var json = JsonSerializer.Serialize(new { kill = new { isSuccess = true, defeatedAt = DateTime.UtcNow.AddHours(1).ToString("o") } });

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(json) });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            await service.CheckBossKills();

            mockRosterService.Verify(s => s.ProcessRoster(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CheckBossKills_ShouldProcessMultipleRosters()
        {
            var roster1 = new BossRoster { RosterId = 1, RaidSlug = "liberation-of-undermine", BossDisplayName = "Vexie and the Geargrinders", InstanceTime = DateTime.UtcNow.AddHours(-2), IsProcessed = false, Participants = new List<Character>() };
            var roster2 = new BossRoster { RosterId = 2, RaidSlug = "liberation-of-undermine", BossDisplayName = "One-Armed Bandit", InstanceTime = DateTime.UtcNow.AddHours(-1), IsProcessed = false, Participants = new List<Character>() };

            var mockRosterService = new Mock<IRosterService>();
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>()))
                             .Returns(new List<BossRoster> { roster1, roster2 });

            var json = JsonSerializer.Serialize(new { kill = new { isSuccess = true, defeatedAt = DateTime.UtcNow.ToString("o") } });

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(json) });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            await service.CheckBossKills();

            mockRosterService.Verify(s => s.Update(It.Is<BossRoster>(r => r.RosterId == 1)), Times.Once);
            mockRosterService.Verify(s => s.Update(It.Is<BossRoster>(r => r.RosterId == 2)), Times.Once);
        }
    }
}
