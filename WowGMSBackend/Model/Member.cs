using System.ComponentModel.DataAnnotations;

namespace WowGMSBackend.Model
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public Rank Rank { get; set; }

        public Member() { }

        public Member(int memberId, string name, string password, Rank rank)
        {
            MemberId = memberId;
            Name = name;
            Password = password;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"Member: ID = {MemberId}, Name = {Name}, Rank = {Rank}";
        }
    }
}
