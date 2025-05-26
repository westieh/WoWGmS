using WowGMSBackend.Model;

namespace WowGMSBackend.Model
{ 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    using WowGMSBackend.Registry;

    public class BossRoster
    {
        [Key]
        public int RosterId { get; set; }
        [Required]
        public List<Character> Participants { get; set; } = new List<Character>();

        public string RaidSlug { get; set; }
        public string BossDisplayName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime InstanceTime { get; set; }
        public bool IsProcessed { get; set; } = false;

        public BossRoster() { }
        public BossRoster(int rosterId, List<Character> participants, string raidSlug, string bossDisplayName, DateTime creationDate, DateTime instanceTime)
        {
            RosterId = rosterId;
            Participants = participants;
            RaidSlug = raidSlug;
            BossDisplayName = bossDisplayName;
            CreationDate = creationDate;
            InstanceTime = instanceTime;
        }
        public Boss? GetBoss() =>
        RaidRegistry.GetBoss(RaidSlug, BossDisplayName);





    }
}
