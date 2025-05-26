using Microsoft.EntityFrameworkCore;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
public class RosterService : IRosterService
{
    private readonly WowDbContext _context;

    public RosterService(WowDbContext context)
    {
        _context = context;
    }

    public void AddCharacterToRoster(int rosterId, Character character)
    {
        var roster = _context.BossRosters
            .Include(r => r.Participants)
            .FirstOrDefault(r => r.RosterId == rosterId);

        if (roster == null) return;

        roster.Participants.Add(character);
        _context.SaveChanges();
    }
    public void RemoveCharacterFromRoster(int rosterId, string characterName, string realmName)
    {
        var roster = _context.BossRosters
            .Include(r => r.Participants)
            .FirstOrDefault(r => r.RosterId == rosterId);

        if (roster == null) return;

        var toRemove = roster.Participants
            .FirstOrDefault(c => c.CharacterName == characterName && c.RealmName.ToString() == realmName);

        if (toRemove != null)
        {
            roster.Participants.Remove(toRemove);
            _context.SaveChanges();
        }
    }
}
