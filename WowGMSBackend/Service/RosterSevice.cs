using Microsoft.EntityFrameworkCore;
using WowGMSBackend.DBContext;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Registry;
using WowGMSBackend.Repository;

namespace WowGMSBackend.Service;

/// <summary>
/// Service responsible for managing raid rosters, including adding/removing characters, 
/// validating rosters, and processing boss kills.
/// </summary>
public class RosterService : IRosterService
{
    private readonly IRosterRepository _rosterRepo;
    private readonly ICharacterService _characterService;

    private const int MaxParticipants = 20;

    /// <summary>
    /// Constructs the RosterService with the required repositories.
    /// </summary>
    public RosterService(IRosterRepository rosterRepo, ICharacterService characterService)
    {
        _rosterRepo = rosterRepo;
        _characterService = characterService;
    }

    /// <summary>
    /// Retrieves all characters not already participating in the given roster.
    /// </summary>
    public List<Character> GetEligibleCharacters(BossRoster roster)
    {
        var existingIds = roster.Participants.Select(p => p.Id).ToHashSet();

        var allCharacters = _characterService.GetCharacters();
        var eligibleCharacters = allCharacters
            .Where(c => !existingIds.Contains(c.Id))
            .ToList();

        return eligibleCharacters;
    }

    /// <summary>
    /// Retrieves all rosters.
    /// </summary>
    public IEnumerable<BossRoster> GetAllRosters()
    {
        return _rosterRepo.GetAll();
    }

    /// <summary>
    /// Adds a character to the specified roster if not already present.
    /// </summary>
    public void AddCharacterToRoster(int rosterId, Character character)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return;

        if (!IsCharacterUnique(roster, character))
            throw new InvalidOperationException($"Character '{character.CharacterName}' is already in the roster.");

        roster.Participants.Add(character);
        _rosterRepo.Update(roster);
    }

    /// <summary>
    /// Retrieves a character by its ID.
    /// </summary>
    public Character? GetCharacterById(int characterId)
    {
        return _characterService.GetCharacter(characterId);
    }

    /// <summary>
    /// Removes a character from the specified roster by their participant ID.
    /// </summary>
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

    /// <summary>
    /// Checks if a character is unique in the given roster.
    /// </summary>
    public bool IsCharacterUnique(BossRoster roster, Character character)
    {
        return !roster.Participants.Any(p => p.CharacterName == character.CharacterName);
    }

    /// <summary>
    /// Processes a roster by marking it as processed and incrementing boss kills for each participant.
    /// </summary>
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

    /// <summary>
    /// Validates if the roster has at least one tank, healer, melee DPS, and ranged DPS.
    /// </summary>
    public bool CheckRosterBalance(BossRoster roster)
    {
        bool hasTank = roster.Participants.Any(p => p.Role == Role.Tank);
        bool hasHealer = roster.Participants.Any(p => p.Role == Role.Healer);
        bool hasRangedDps = roster.Participants.Any(p => p.Role == Role.RangedDPS);
        bool hasMeleeDps = roster.Participants.Any(p => p.Role == Role.MeleeDPS);

        return hasTank && hasHealer && hasRangedDps && hasMeleeDps;
    }

    /// <summary>
    /// Retrieves all rosters that are unprocessed and have an instance time before the given date.
    /// </summary>
    public IEnumerable<BossRoster> GetUnprocessedRostersBefore(DateTime utcNow)
    {
        return _rosterRepo
            .GetAll()
            .Where(r => !r.IsProcessed && r.InstanceTime <= utcNow)
            .OrderByDescending(r => r.InstanceTime);
    }

    /// <summary>
    /// Updates a roster with new information.
    /// </summary>
    public BossRoster? Update(BossRoster updated)
    {
        return _rosterRepo.Update(updated);
    }

    /// <summary>
    /// Deletes a roster by its ID.
    /// </summary>
    public void Delete(int id)
    {
        _rosterRepo.Delete(id);
    }

    /// <summary>
    /// Retrieves a roster by its ID.
    /// </summary>
    public BossRoster? GetRosterById(int rosterId)
    {
        return _rosterRepo.GetById(rosterId);
    }

    /// <summary>
    /// Creates a new roster based on selected raid and boss information.
    /// </summary>
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

    /// <summary>
    /// Retrieves all rosters that have assigned bosses.
    /// </summary>
    public List<BossRoster> GetRostersWithBosses()
    {
        return _rosterRepo.GetAll().Where(r => r.BossDisplayName != null).ToList();
    }

    /// <summary>
    /// Retrieves all upcoming rosters sorted by instance time.
    /// </summary>
    public List<BossRoster> GetUpcomingRosters()
    {
        return _rosterRepo.GetAll()
            .Where(r => r.InstanceTime >= DateTime.Now)
            .OrderBy(r => r.InstanceTime)
            .ToList();
    }

    /// <summary>
    /// Updates the scheduled instance time for a given roster.
    /// </summary>
    public void UpdateRosterTime(int rosterId, DateTime newTime)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return;
        roster.InstanceTime = newTime;
        _rosterRepo.Update(roster);
    }

    /// <summary>
    /// Checks if a roster has reached its maximum participant capacity.
    /// </summary>
    public bool IsRosterAtCapacity(int rosterId)
    {
        var roster = _rosterRepo.GetById(rosterId);
        if (roster == null) return true; // Assume full if roster doesn't exist

        return roster.Participants.Count >= MaxParticipants;
    }
}
