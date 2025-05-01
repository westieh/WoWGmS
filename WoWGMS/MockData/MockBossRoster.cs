namespace WoW.MockData;
using WoW.Model;

    public class MockBossRoster
    {
    private static List<BossRoster> bossRosters = new List<BossRoster>()
    {
        //new BossRoster(),
        //new BossRoster(),
        //new BossRoster()
    };
        public static List<BossRoster> GetBossRoster() {  return bossRosters; }
}
