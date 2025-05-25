using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowGMSBackend.Model
{
    public class Character
    {
        [Key]
        
        public int Id { get; set; } //Påkrævet for at migration fungerer
        [Required]
        [MaxLength(12)]
        public string CharacterName { get; set; }
        [Required]
        [MaxLength(50)]
        public string RealmName { get; set; }
        public Class Class { get; set; }
        public Role Role { get; set; }
        public Member Owner { get; set; }
        
        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        [NotMapped]
        [Required]
        public Dictionary<string, int> BossKills { get; set; } = new();

        public Character() 
        {
            BossKills = new Dictionary<string, int>();
        }
        public Character(string _name, Class _class, Role _role)
        {
            CharacterName = _name;
            Class = _class;
            Role = _role;
        }


        public override string ToString()
        {
            return $"Character: Name = {CharacterName}, Class = {Class}, Role = {Role}";
        }
        public void IncrementBossKill(string bossSlug)
        {
            if (BossKills.ContainsKey(bossSlug))
                BossKills[bossSlug]++;
            else
                BossKills[bossSlug] = 1;

        }



    }
}
