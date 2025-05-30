using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WowGMSBackend.Registry;

namespace WowGMSBackend.Model
{
    public class BossRoster
    {
        // Primary key for the BossRoster entity
        [Key]
        public int RosterId { get; set; }

        // Navigation property representing the list of Characters assigned to this roster
        public List<Character> Participants { get; set; } = new List<Character>();

        // Slug identifying the Raid (e.g., raid identifier used for API calls or internal mapping)
        public string RaidSlug { get; set; }

        // Human-readable display name for the Boss
        public string BossDisplayName { get; set; }

        // Slug uniquely identifying the Boss (e.g., for API use)
        public string BossSlug { get; set; }

        // Timestamp when the Roster was created (for auditing or tracking purposes)
        public DateTime CreationDate { get; set; }

        // Scheduled instance time for the raid (used for scheduling and event management)
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime InstanceTime { get; set; }

        // Boolean flag indicating if the Roster has been processed (e.g., if the raid has been completed)
        public bool IsProcessed { get; set; } = false;

        // Default constructor for EF and serialization
        public BossRoster() { }

        // Parameterized constructor for easier instantiation with predefined values
        public BossRoster(int rosterId, List<Character> participants, string raidSlug, string bossDisplayName, DateTime creationDate, DateTime instanceTime)
        {
            RosterId = rosterId;
            Participants = participants;
            RaidSlug = raidSlug;
            BossDisplayName = bossDisplayName;
            CreationDate = creationDate;
            InstanceTime = instanceTime;
        }

        // Helper method to retrieve the Boss object based on the RaidSlug and BossDisplayName
        public Boss? GetBoss() =>
            RaidRegistry.GetBossByDisplayName(RaidSlug, BossDisplayName);
    }
}
