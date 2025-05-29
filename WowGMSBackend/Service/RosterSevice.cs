using Microsoft.EntityFrameworkCore;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Registry;
using WowGMSBackend.Repository;

namespace WowGMSBackend.Service;
public class RosterService : IRosterService
{
    private readonly IRosterRepository _rosterRepo;
    private readonly ICharacterService _characterService;

    private const int MaxParticipants = 20;
    public List<Character> GetEligibleCharacters(BossRoster roster)
    {
        var existingIds = roster.Participants.Select(p => p.Id).ToHashSet();

        var allCharacters = _characterService.GetCharacters();
        var eligibleCharacters = allCharacters
            .Where(c => !existingIds.Contains(c.Id))
            .ToList();

        return eligibleCharacters;
    }
    public RosterService(IRosterRepository rosterRepo, ICharacterService characterService)
    {
        _rosterRepo = rosterRepo;
        _characterService = characterService;
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
    public Character? GetCharacterById(int characterId)
    {
        return _characterService.GetCharacter(characterId);
    }
    public void RemoveCharacterFromRoster(int rosterId, int participantId)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return;

        var participant = roster.Participants.FirstOrDefault(p => p.Id == participantId);
        if (participant != null)
        {
            roster.Participants.Remove(participant);
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
    public BossRoster? GetRosterById(int rosterId)
    {
        return _rosterRepo.GetById(rosterId);
    }
    public void CreateRoster(BossRoster newRoster, string raidSlug, string bossSlug)
    {
        if (string.IsNullOrEmpty(raidSlug) || string.IsNullOrEmpty(bossSlug))
            throw new Exception("Missing raid or boss selection.");

        var boss = RaidRegistry.GetBossesForRaid(raidSlug)
                               .FirstOrDefault(b => b.Slug == bossSlug);
        if (boss == null)
            throw new Exception("Invalid boss selected");

        newRoster.RaidSlug = raidSlug;
        newRoster.BossSlug = boss.Slug;
        newRoster.BossDisplayName = boss.DisplayName;
        newRoster.CreationDate = DateTime.Now;

        if (newRoster.InstanceTime == default)
            newRoster.InstanceTime = DateTime.Now.AddHours(1);

        _rosterRepo.Add(newRoster);
    }
    public List<BossRoster> GetRostersWithBosses()
    {
        return _rosterRepo.GetAll().Where(r => r.BossDisplayName != null).ToList();
    }

    public List<BossRoster> GetUpcomingRosters()
    {
        return _rosterRepo.GetAll()
            .Where(r => r.InstanceTime >= DateTime.Now)
            .OrderBy(r => r.InstanceTime)
            .ToList();
    }
    public void UpdateRosterTime(int rosterId, DateTime newTime)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return;
        roster.InstanceTime = newTime;
        _rosterRepo.Update(roster);
    }

    public bool IsRosterAtCapacity(int rosterId)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return true; // block operation if roster doesn't exist

        return roster.Participants.Count >= MaxParticipants;
    }

}
