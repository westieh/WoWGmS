using WowGMSBackend.Model;

namespace WowGMSBackend.Interfaces
{
    public interface IRosterService
    {
        void AddCharacterToRoster(int rosterId, Character character);
        void RemoveCharacterFromRoster(int rosterId, int characterId);
        bool IsCharacterUnique(BossRoster roster, Character character);
        bool CheckRosterBalance(BossRoster roster);
        void ProcessRoster(int rosterId);
        IEnumerable<BossRoster> GetUnprocessedRostersBefore(DateTime utcNow);
        BossRoster? Update(BossRoster updated);
        IEnumerable<BossRoster> GetAllRosters();
        void Delete(int id);
        void CreateRoster(BossRoster roster, string raidSlug, string bossSlug);
        BossRoster? GetRosterById(int rosterId);
        List<BossRoster> GetRostersWithBosses();
        List<BossRoster> GetUpcomingRosters();
        void UpdateRosterTime(int rosterId, DateTime newTime);
        Character? GetCharacterById(int characterId);
        List<Character> GetEligibleCharacters(BossRoster roster);
        bool IsRosterAtCapacity(int rosterId);
    }
}