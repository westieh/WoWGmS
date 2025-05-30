using System;
using System.Collections.Generic;
using System.Linq;
using WowGMSBackend.Model;

namespace WowGMSBackend.Registry
{
    /// <summary>
    /// Static registry holding predefined raids and their associated bosses.
    /// Used for raid and boss lookups within the application.
    /// </summary>
    public static class RaidRegistry
    {
        /// <summary>
        /// Hardcoded list of raids and their bosses.
        /// Acts as a local data source for raid-boss relationships.
        /// </summary>
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

        /// <summary>
        /// Retrieves a boss from a specific raid by matching its display name (case-insensitive).
        /// Returns null if raid or boss is not found.
        /// </summary>
        public static Boss? GetBossByDisplayName(string raidSlug, string displayName)
        {
            var raid = Raids.FirstOrDefault(r => r.Slug.Equals(raidSlug, StringComparison.OrdinalIgnoreCase));
            if (raid == null) return null;

            return raid.Bosses.FirstOrDefault(b => b.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Retrieves all bosses for a given raid slug (case-insensitive).
        /// Returns an empty list if raid is not found.
        /// </summary>
        public static List<Boss> GetBossesForRaid(string raidSlug)
        {
            var raid = Raids.FirstOrDefault(r => r.Slug.Equals(raidSlug, StringComparison.OrdinalIgnoreCase));
            return raid?.Bosses ?? new List<Boss>();
        }
    }
}
