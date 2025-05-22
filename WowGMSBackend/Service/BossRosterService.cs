namespace WowGMSBackend.Service;
using WowGMSBackend.Model;
using WowGMSBackend.MockData;

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
