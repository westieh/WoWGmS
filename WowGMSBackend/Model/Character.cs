using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WoW.Model
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
        public ServerName RealmName { get; set; }
        public Class Class { get; set; }
        public Role Role { get; set; }
        public Member Member { get; set; }
        
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        [NotMapped]
        [Required]
        public Dictionary<BossName, int> BossKills { get; set; }

        public Character() 
        {
            BossKills = Enum.GetValues(typeof(BossName))
                    .Cast<BossName>()
                    .ToDictionary(boss => boss, boss => 0); 
        }
        public Character(string _name, Class _class, Role _role, ServerName _realmName)
        {
            CharacterName = _name;
            Class = _class;
            Role = _role;
            RealmName = _realmName;
        }


        public override string ToString()
        {
            return $"Character: Name = {CharacterName}, Class = {Class}, Role = {Role}";
        }
        public void IncrementBossKill(BossName boss)
        {
            if (BossKills.ContainsKey(boss))
                BossKills[boss]++;
        }



    }
}
