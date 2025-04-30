namespace WoW.Model
{
    public class BossRoster
    {
        public int RosterId { get; set; }
        public List<Character> Participants { get; set; } = new List<Character>();
        public string BossName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime InstanceTime { get; set; }

       public BossRoster() { }
        public BossRoster(int rosterId, List<Character> participants, string bossName, DateTime creationDate, DateTime instanceTime)
        {
            RosterId = rosterId;
            Participants = participants;
            BossName = bossName;
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
