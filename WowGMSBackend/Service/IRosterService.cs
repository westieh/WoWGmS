using WowGMSBackend.Model;

namespace WowGMSBackend.Service
{
    public interface IRosterService
    {
        void AddCharacterToRoster(int rosterId, Character character);
        void RemoveCharacterFromRoster(int rosterId, string characterName, string realmName);
    }
}