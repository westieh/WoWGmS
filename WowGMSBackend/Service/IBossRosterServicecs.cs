using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public interface IBossRosterServicecs
    {
        void AddToRoster(BossRoster roster, Character character);

        void RemoveFromRoster(BossRoster roster, string characterName, string realmName);
    }
}
