namespace WoW.Service;
using WoW.Model;
using WoW.MockData;

    public class BossRosterService : IBossRosterServicecs
    {
    public List<BossRoster> _bossRosters;

    public BossRosterService()
    {
        _bossRosters = MockBossRoster.GetBossRoster();
    }

    public void AddToRoster(BossRoster bossRoster, Character character)
    {

    }
    public void RemoveFromRoster(BossRoster bossRoster, string characterName, string realmName)
    {

    }
}
