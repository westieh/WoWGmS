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

        public void AddToRoster(Character character)
        {
            if (Participants == null)
            {
                Participants = new List<Character>();
            }
            Participants.Add(character);
        }

        public bool RemoveFromRoster(Character character)
        {
            if (Participants.Contains(character))
            {
                Participants.Remove(character);
                return true;
            }
            return false;
        }

        public bool CheckRosterBalance()
        {
            bool hasTank = Participants.Any(p => p.Role == Role.Tank);
            bool hasHealer = Participants.Any(p => p.Role == Role.Healer);
            bool hasRangedDps = Participants.Any(p => p.Role == Role.RangedDPS);
            bool hasMeleeDps = Participants.Any(p => p.Role == Role.MeleeDPS);

            return hasTank && hasHealer && hasRangedDps && hasMeleeDps;
        }

        private void UniqueMemberCheck(Character character)
        {
            if (Participants.Any(p => p.CharacterName == character.CharacterName))
            {
                throw new InvalidOperationException($"Member with the character name {character.CharacterName} is already in the roster.");
            }
        }
    }
}
