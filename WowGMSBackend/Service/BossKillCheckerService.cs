using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using WowGMSBackend.Repository;
using WowGMSBackend.Model;
using System.Text.Json;
using WowGMSBackend.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace WowGMSBackend.Service
{
    public class BossKillCheckerService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public BossKillCheckerService(IHttpClientFactory httpClientFactory, IServiceScopeFactory socpeFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _scopeFactory = socpeFactory;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckBossKills();
                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
        private async Task CheckBossKills()
        {
            using var scope = _scopeFactory.CreateScope();
            var _rosterRepo = scope.ServiceProvider.GetRequiredService<IRosterRepository>();
            var client = _httpClientFactory.CreateClient("RaiderIO");

            var roster = _rosterRepo
                .GetAll()
                .Where(r => !r.IsProcessed && r.InstanceTime <= DateTime.UtcNow)
                .OrderByDescending(r => r.InstanceTime)
                .FirstOrDefault();

            if (roster != null)
            {
                var boss = roster.GetBoss();
                if (boss == null) return;

                var url = UrlBuilder.BuildBossKillUrl(_config, boss.Slug, roster.RaidSlug); var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<BossKillResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Kill?.IsSuccess == true && data.Kill.DefeatedAt <= DateTime.UtcNow)
                    _rosterRepo.MarkAsProcessed(roster.RosterId);
            }

        }
    }
}
