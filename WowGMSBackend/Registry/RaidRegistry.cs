using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowGMSBackend.Model;
namespace WowGMSBackend.Registry
{
    public static class RaidRegistry
    {
        public static readonly List<Raid> Raids = new()
        {
            new Raid
            {
                Name = "Liberation of Undermine",
                Slug = "liberation-of-undermine",
                Bosses = new List<Boss>
                {
                    new Boss { DisplayName = "Vexie and the Geargrinders", Slug = "vexie-and-the-geargrinders" },
                    new Boss { DisplayName = "Cauldron of Carnage", Slug = "cauldron-of-carnage" },
                    new Boss { DisplayName = "Rik Reverb", Slug = "rik-reverb" },
                    new Boss { DisplayName = "Stix Bunkjunker", Slug = "stix-bunkjunker" },
                    new Boss { DisplayName = "Sprocketmonger Lockenstock", Slug = "sprocketmonger-lockenstock" },
                    new Boss { DisplayName = "One-Armed Bandit", Slug = "onearmed-bandit" },
                    new Boss { DisplayName = "MugZee, Heads of Security", Slug = "mugzee-heads-of-security" },
                    new Boss { DisplayName = "Chrome King Gallywix", Slug = "chrome-king-gallywix" }
                }
            }

        };
        public static Boss? GetBossByDisplayName(string raidSlug, string displayName)
        {
            var raid = Raids.FirstOrDefault(r => r.Slug.Equals(raidSlug, StringComparison.OrdinalIgnoreCase));
            if (raid == null) return null;

            return raid.Bosses.FirstOrDefault(b => b.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        public static List<Boss> GetBossesForRaid(string raidSlug)
        {
            var raid = Raids.FirstOrDefault(r => r.Slug.Equals(raidSlug, StringComparison.OrdinalIgnoreCase));
            return raid?.Bosses ?? new List<Boss>();
        }
    }
}
