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
        // Subclass to expose protected CheckBossKills method for testing
        private class TestableBossKillCheckerService : BossKillCheckerService
        {
            public TestableBossKillCheckerService(
                IHttpClientFactory httpClientFactory,
                IServiceScopeFactory scopeFactory,
                IConfiguration config)
                : base(httpClientFactory, scopeFactory, config) { }

            public new async Task CheckBossKills() => await base.CheckBossKills();
        }

        // Helper method to build a mock configuration for the service
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
        public async Task CheckBossKills_ShouldUpdateRoster_WhenKillConfirmed()
        {
            // Arrange
            // Create a test character and roster with 1 participant
            var testCharacter = new Character { CharacterName = "TestChar" };
            var testRoster = new BossRoster
            {
                RosterId = 1,
                RaidSlug = "liberation-of-undermine",
                BossDisplayName = "Vexie and the Geargrinders",
                InstanceTime = DateTime.UtcNow.AddHours(-1),
                IsProcessed = false,
                Participants = new List<Character> { testCharacter }
            };

            // Mock the roster service to return the test roster
            var mockRosterService = new Mock<IRosterService>();
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>())).Returns(new List<BossRoster> { testRoster });

            // Mock the API response to return a successful boss kill within time window
            var json = JsonSerializer.Serialize(new { kill = new { isSuccess = true, defeatedAt = DateTime.UtcNow.ToString("o") } });
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Setup HttpClient and dependencies
            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            // Act
            await service.CheckBossKills();

            // Assert
            // Verify roster was marked as processed and Update was called
            mockRosterService.Verify(s => s.Update(It.Is<BossRoster>(r => r.RosterId == 1 && r.IsProcessed == true)), Times.Once);
            // Verify the character received a boss kill increment
            Assert.Single(testCharacter.BossKills);
            Assert.Equal(1, testCharacter.BossKills[0].KillCount);
        }

        [Fact]
        public async Task CheckBossKills_ShouldNotUpdateRoster_WhenApiReturns404()
        {
            // Arrange
            // Setup a test roster
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
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>())).Returns(new List<BossRoster> { testRoster });

            // Mock API to return 404 Not Found
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            // Act
            await service.CheckBossKills();

            // Assert
            // Verify no update was called since API call failed
            mockRosterService.Verify(s => s.Update(It.IsAny<BossRoster>()), Times.Never);
        }

        [Fact]
        public async Task CheckBossKills_ShouldNotUpdateRoster_WhenKillIsUnsuccessful()
        {
            // Arrange
            // Setup a test roster
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
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>())).Returns(new List<BossRoster> { testRoster });

            // Mock API response where the kill was not successful
            var json = JsonSerializer.Serialize(new { kill = new { isSuccess = false, defeatedAt = DateTime.UtcNow.ToString("o") } });
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            // Act
            await service.CheckBossKills();

            // Assert
            // Verify no update was called because kill was not successful
            mockRosterService.Verify(s => s.Update(It.IsAny<BossRoster>()), Times.Never);
        }

        [Fact]
        public async Task CheckBossKills_ShouldNotUpdateRoster_WhenDefeatedAtIsInFuture()
        {
            // Arrange
            // Setup a test roster
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
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>())).Returns(new List<BossRoster> { testRoster });

            // Mock API response where kill time is in the future (invalid)
            var json = JsonSerializer.Serialize(new { kill = new { isSuccess = true, defeatedAt = DateTime.UtcNow.AddHours(13).ToString("o") } });
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            // Act
            await service.CheckBossKills();

            // Assert
            // Verify no update was called because kill time is outside allowed window
            mockRosterService.Verify(s => s.Update(It.IsAny<BossRoster>()), Times.Never);
        }

        [Fact]
        public async Task CheckBossKills_ShouldUpdateMultipleRosters_WhenKillsAreConfirmed()
        {
            // Arrange
            // Setup two test rosters
            var roster1 = new BossRoster
            {
                RosterId = 1,
                RaidSlug = "liberation-of-undermine",
                BossDisplayName = "Vexie and the Geargrinders",
                InstanceTime = DateTime.UtcNow.AddHours(-2),
                IsProcessed = false,
                Participants = new List<Character>()
            };

            var roster2 = new BossRoster
            {
                RosterId = 2,
                RaidSlug = "liberation-of-undermine",
                BossDisplayName = "One-Armed Bandit",
                InstanceTime = DateTime.UtcNow.AddHours(-1),
                IsProcessed = false,
                Participants = new List<Character>()
            };

            var mockRosterService = new Mock<IRosterService>();
            mockRosterService.Setup(s => s.GetUnprocessedRostersBefore(It.IsAny<DateTime>())).Returns(new List<BossRoster> { roster1, roster2 });

            // Mock API response where kill is successful and valid
            var json = JsonSerializer.Serialize(new { kill = new { isSuccess = true, defeatedAt = DateTime.UtcNow.ToString("o") } });
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var client = new HttpClient(handler.Object);
            var mockHttpFactory = new Mock<IHttpClientFactory>();
            mockHttpFactory.Setup(f => f.CreateClient("RaiderIO")).Returns(client);

            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScope.Setup(s => s.ServiceProvider.GetService(typeof(IRosterService))).Returns(mockRosterService.Object);
            mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

            var config = BuildTestConfig();
            var service = new TestableBossKillCheckerService(mockHttpFactory.Object, mockScopeFactory.Object, config);

            // Act
            await service.CheckBossKills();

            // Assert
            // Verify both rosters are processed
            mockRosterService.Verify(s => s.Update(It.Is<BossRoster>(r => r.RosterId == 1)), Times.Once);
            mockRosterService.Verify(s => s.Update(It.Is<BossRoster>(r => r.RosterId == 2)), Times.Once);
        }
    }
}
