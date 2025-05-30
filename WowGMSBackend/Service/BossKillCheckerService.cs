using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using WowGMSBackend.Model;
using System.Text.Json;
using WowGMSBackend.Helpers;
using Microsoft.Extensions.DependencyInjection;
using WowGMSBackend.Interfaces;

namespace WowGMSBackend.Service
{
    /// <summary>
    /// Background service that periodically checks for boss kills via the RaiderIO API
    /// and updates rosters accordingly.
    /// </summary>
    public class BossKillCheckerService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes the service with required dependencies.
        /// </summary>
        public BossKillCheckerService(IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
            _config = config;
        }

        /// <summary>
        /// Main loop for the background service. It periodically checks for boss kills every hour.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckBossKills();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        /// <summary>
        /// Checks whether the boss kill timestamp falls within an acceptable time window
        /// relative to the scheduled instance time.
        /// </summary>
        /// <param name="instanceTime">The scheduled start time of the raid instance.</param>
        /// <param name="killTime">The actual boss kill time retrieved from the API.</param>
        /// <returns>True if the kill happened within an acceptable window; otherwise, false.</returns>
        private bool IsKillInWindow(DateTime instanceTime, DateTime killTime)
        {
            var earlyWindow = instanceTime.AddMinutes(-30);  // Accept kills up to 30 minutes early.
            var lateWindow = instanceTime.AddHours(12);      // Accept kills up to 12 hours late.
            return killTime >= earlyWindow && killTime <= lateWindow;
        }

        /// <summary>
        /// Core logic that fetches rosters pending validation, queries the RaiderIO API
        /// for boss kill information, and updates the roster and characters if the kill is confirmed.
        /// </summary>
        protected internal virtual async Task CheckBossKills()
        {
            using var scope = _scopeFactory.CreateScope();
            var _rosterRepo = scope.ServiceProvider.GetRequiredService<IRosterService>();

            // Fetch all unprocessed rosters that have instance times earlier than now
            var rosters = _rosterRepo.GetUnprocessedRostersBefore(DateTime.UtcNow);

            foreach (var roster in rosters)
            {
                var boss = roster.GetBoss();
                if (boss == null) continue;

                // Construct the API URL
                var url = UrlBuilder.BuildBossKillUrl(_config, boss.Slug, roster.RaidSlug);
                var client = _httpClientFactory.CreateClient("RaiderIO");

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<BossKillResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Kill == null) continue;

                // Check if the boss kill was successful and falls within the time window
                if (data.Kill.IsSuccess && IsKillInWindow(roster.InstanceTime, data.Kill.DefeatedAt))
                {
                    // Increment boss kill count for each participant
                    foreach (var character in roster.Participants)
                    {
                        character.IncrementBossKill(boss.Slug);
                    }

                    roster.IsProcessed = true;
                    _rosterRepo.Update(roster);
                }
            }
        }
    }
}
