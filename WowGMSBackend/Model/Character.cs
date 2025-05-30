using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WowGMSBackend.Model
{
    public class Character
    {
        // Primary key for the Character entity
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Required for EF Core migrations to work

        // Character name with a maximum length constraint
        [Required]
        [MaxLength(12)]
        public string CharacterName { get; set; }

        // Server/Realm where the character is located
        [Required]
        public ServerName RealmName { get; set; }

        // Class enum representing the character's class (e.g., Warrior, Mage)
        public Class Class { get; set; }

        // Role enum representing the character's role (e.g., Tank, Healer)
        public Role Role { get; set; }

        // Navigation property to the Member entity; validation skipped
        [ValidateNever]
        public Member Member { get; set; }

        // Foreign key to the associated Member
        [ForeignKey("Member")]
        public int MemberId { get; set; }

        // List of BossKill records associated with this character
        public List<BossKill> BossKills { get; set; } = new();

        // Default constructor for EF and serialization
        public Character() { }

        // Parameterized constructor for easier instantiation
        public Character(string _name, Class _class, Role _role, ServerName _realmName)
        {
            CharacterName = _name;
            Class = _class;
            Role = _role;
            RealmName = _realmName;
        }

        // String representation of the character for logging/debugging
        public override string ToString()
        {
            return $"Character: Name = {CharacterName}, Class = {Class}, Role = {Role}";
        }

        // Method to increment the kill count for a specific boss
        public void IncrementBossKill(string bossSlug)
        {
            var existing = BossKills.FirstOrDefault(bk => bk.BossSlug == bossSlug);
            if (existing != null)
            {
                existing.KillCount++;
            }
            else
            {
                BossKills.Add(new BossKill
                {
                    BossSlug = bossSlug,
                    KillCount = 1
                });
            }
        }
    }
}
