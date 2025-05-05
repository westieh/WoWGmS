using WoW.Model;

namespace WoW.Service
{
    public interface IBossRosterServicecs
    {
        void AddToRoster(BossRoster roster, Character character);

        void RemoveFromRoster(BossRoster roster, string characterName, string realmName);
    }
}
