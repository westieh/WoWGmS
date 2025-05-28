using Microsoft.EntityFrameworkCore;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

namespace WowGMSBackend.Service;
public class RosterService : IRosterService
{
    private readonly IRosterRepository _rosterRepo;

    public RosterService(IRosterRepository rosterRepo)
    {
        _rosterRepo = rosterRepo;
    }
    public IEnumerable<BossRoster> GetAllRosters()
    {
        return _rosterRepo.GetAll();
    }
    public void AddCharacterToRoster(int rosterId, Character character)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return;

        if (!IsCharacterUnique(roster, character))
            throw new InvalidOperationException($"Character '{character.CharacterName}' is already in the roster.");

        roster.Participants.Add(character);
        _rosterRepo.Update(roster);
    }
    public void RemoveCharacterFromRoster(int rosterId, string characterName, string realmName)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return;

        var toRemove = roster.Participants.FirstOrDefault(c =>
            c.CharacterName == characterName &&
            c.RealmName.ToString() == realmName);

        if (toRemove != null)
        {
            roster.Participants.Remove(toRemove);
            _rosterRepo.Update(roster);
        }
    }
    public bool IsCharacterUnique(BossRoster roster, Character character)
    {
        return !roster.Participants.Any(p => p.CharacterName == character.CharacterName);
    }
    public void ProcessRoster(int rosterId)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null || roster.IsProcessed) return;

        var boss = roster.GetBoss();
        if (boss == null) return;

        foreach (var character in roster.Participants)
        {
            character.IncrementBossKill(boss.Slug);
        }

        roster.IsProcessed = true;
        _rosterRepo.Update(roster);
    }
    public bool CheckRosterBalance(BossRoster roster)
    {
        bool hasTank = roster.Participants.Any(p => p.Role == Role.Tank);
        bool hasHealer = roster.Participants.Any(p => p.Role == Role.Healer);
        bool hasRangedDps = roster.Participants.Any(p => p.Role == Role.RangedDPS);
        bool hasMeleeDps = roster.Participants.Any(p => p.Role == Role.MeleeDPS);

        return hasTank && hasHealer && hasRangedDps && hasMeleeDps;
    }
    public IEnumerable<BossRoster> GetUnprocessedRostersBefore(DateTime utcNow)
    {
        return _rosterRepo
            .GetAll()
            .Where(r => !r.IsProcessed && r.InstanceTime <= utcNow)
            .OrderByDescending(r => r.InstanceTime);
    }
    public BossRoster? Update(BossRoster updated)
    {
        return _rosterRepo.Update(updated);
    }
    public void Delete(int id)
    {
        _rosterRepo.Delete(id);
    }
}
