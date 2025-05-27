using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;

namespace WowGMSBackend.Helpers
{
    public static class UrlBuilder
    {
        public static string BuildBossKillUrl(IConfiguration config, string bossSlug, string raidSlug)
        {
            var baseUrl = config["RaiderIO:ApiBase"]?.TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Missing RaiderIO:ApiBase in config.");

            return $"{baseUrl}/api/v1/guilds/boss-kill" +
                   $"?region={config["RaiderIO:Region"]}" +
                   $"&realm={config["RaiderIO:Realm"]}" +
                   $"&guild={config["RaiderIO:Guild"]}" +
                   $"&raid={raidSlug}" +
                   $"&boss={bossSlug}" +
                   $"&difficulty={config["RaiderIO:Difficulty"]}" +
                   $"&access_key={config["RaiderIO:AccessKey"]}";
        }
    }
}
