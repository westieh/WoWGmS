namespace WoW.Model
{
    public class BossRoster
    {
        public int RosterId { get; set; }
        public List<Character> Participants { get; set; } = new List<Character>();
        public BossName BossName {  get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime InstanceTime { get; set; }
        public bool IsProcessed { get; set; } = false;

        public BossRoster() { }
        public BossRoster(int rosterId, List<Character> participants, BossName boss, DateTime creationDate, DateTime instanceTime)
        {
            RosterId = rosterId;
            Participants = participants;
            BossName = boss;
            CreationDate = creationDate;
            InstanceTime = instanceTime;
        }

        public void AddToRoster(Character character)
        {
            if (Participants == null)
            {
                Participants = new List<Character>();
            }
            Participants.Add(character);
        }
        
    }
}
