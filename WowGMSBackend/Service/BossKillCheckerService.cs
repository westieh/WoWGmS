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

namespace WowGMSBackend.Service
{
    public class BossKillCheckerService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRosterRepository _rosterRepo;
        private readonly IConfiguration _config;

        public BossKillCheckerService(IHttpClientFactory httpClientFactory, IRosterRepository rosterRepo, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _rosterRepo = rosterRepo;
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
            var client = _httpClientFactory.CreateClient("RaiderIO");

            var roster = _rosterRepo
                .GetAll()
                .Where(r => !r.IsProcessed && r.InstanceTime <= DateTime.UtcNow)
                .OrderByDescending(r => r.InstanceTime)
                .FirstOrDefault();

            if (roster != null)
            {
                var url = BuildUrl(roster.BossName.ToString());
                var response = await client.GetAsync(url);
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
        private string BuildUrl(string bossSlug)
        {
            return $"/api/v1/guilds/boss-kill" +
                   $"?region={_config["RaiderIO:Region"]}" +
                   $"&realm={_config["RaiderIO:Realm"]}" +
                   $"&guild={_config["RaiderIO:Guild"]}" +
                   $"&raid={_config["RaiderIO:Raid"]}" +
                   $"&boss={bossSlug}" +
                   $"&difficulty={_config["RaiderIO:Difficulty"]}" +
                   $"&access_key={_config["RaiderIO:AccessKey"]}";
        }
    }
}
