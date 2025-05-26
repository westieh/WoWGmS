using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface IRosterService
    {
        void AddCharacterToRoster(int rosterId, Character character);
        void RemoveCharacterFromRoster(int rosterId, string characterName, string realmName);
        bool IsCharacterUnique(BossRoster roster, Character character);
        void ProcessRoster(int rosterId);
        IEnumerable<BossRoster> GetUnprocessedRostersBefore(DateTime utcNow);

    }
}