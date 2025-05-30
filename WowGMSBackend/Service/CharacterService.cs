using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WowGMSBackend.Interfaces;
using WowGMSBackend.Model;
using WowGMSBackend.Repository;

namespace WowGMSBackend.Service
{
    /// <summary>
    /// Service responsible for operations related to Characters,
    /// including creating characters, assigning boss kills, and updating character data.
    /// </summary>
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepo _repo;

        /// <summary>
        /// Initializes the CharacterService with a character repository dependency.
        /// </summary>
        public CharacterService(ICharacterRepo repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Creates a new character and attaches initial boss kill data based on provided inputs.
        /// Each entry in killInputs represents a boss slug and the number of kills.
        /// </summary>
        /// <param name="character">Character object to be created.</param>
        /// <param name="killInputs">Dictionary mapping boss slugs to kill counts.</param>
        /// <param name="memberId">The ID of the member who owns the character.</param>
        public void CreateCharacterWithKills(Character character, Dictionary<string, int> killInputs, int memberId)
        {
            character.MemberId = memberId;
            character.BossKills ??= new List<BossKill>();

            foreach (var kvp in killInputs)
            {
                var bossSlug = kvp.Key;
                var killCount = kvp.Value;

                character.BossKills.Add(new BossKill
                {
                    BossSlug = bossSlug,
                    KillCount = killCount,
                    Character = character
                });
            }

            _repo.AddCharacter(character);
        }

        /// <summary>
        /// Adds a new character to the repository.
        /// </summary>
        public Character AddCharacter(Character character)
        {
            return _repo.AddCharacter(character);
        }

        /// <summary>
        /// Retrieves a character by its ID.
        /// </summary>
        public Character? GetCharacter(int id)
        {
            return _repo.GetCharacter(id);
        }

        /// <summary>
        /// Retrieves all characters with their associated member and boss kill data.
        /// </summary>
        public List<Character> GetCharacters()
        {
            return _repo.GetCharacters();
        }

        /// <summary>
        /// Updates an existing character with new information.
        /// </summary>
        public Character? UpdateCharacter(int id, Character updated)
        {
            return _repo.UpdateCharacter(id, updated);
        }

        /// <summary>
        /// Deletes a character by its ID.
        /// </summary>
        public Character? DeleteCharacter(int id)
        {
            return _repo.DeleteCharacter(id);
        }

        /// <summary>
        /// Adds a new boss kill record to the character.
        /// </summary>
        public void AddBossKill(BossKill bossKill)
        {
            _repo.AddBossKill(bossKill);
        }

        /// <summary>
        /// Increments the kill count for a given boss for a character.
        /// If the boss kill does not exist, it will be created.
        /// </summary>
        public void IncrementBossKill(int characterId, string bossSlug)
        {
            var character = _repo.GetCharacter(characterId);
            character?.IncrementBossKill(bossSlug);
        }

        /// <summary>
        /// Retrieves all characters belonging to a specific member by member ID.
        /// </summary>
        public List<Character> GetCharactersByMemberId(int memberId)
        {
            return _repo.GetCharactersByMemberId(memberId);
        }

        /// <summary>
        /// Retrieves all characters from the database.
        /// </summary>
        public List<Character> GetAllCharacters()
        {
            return _repo.GetCharacters();
        }


    }
}
