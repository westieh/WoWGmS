namespace WoW.Model
{
    public class EventRepo
    {
        public int EventId { get; set; }
        public List<BossRoster> BossRoster { get; set; }
        public DateTime EventDate { get; set; }
    }
}
