using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WowGMSBackend.Model
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } //Påkrævet for at migration fungerer
        [Required]
        [MaxLength(12)]
        public string CharacterName { get; set; }
        [Required]
        public ServerName RealmName { get; set; }
        public Class Class { get; set; }
        public Role Role { get; set; }
        [ValidateNever]
        public Member Member { get; set; }
        
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public List<BossKill> BossKills { get; set; } = new();


        public Character() 
        {
            
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
