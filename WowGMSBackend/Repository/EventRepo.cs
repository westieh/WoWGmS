using WoW.Model;

namespace WoWGMS.Repository
{
    public class EventRepo
    {
        public int EventId { get; set; }
        public List<BossRoster> BossRoster { get; set; }
        public DateTime EventDate { get; set; }
    }
}
