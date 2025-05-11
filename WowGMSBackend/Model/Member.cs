using System.ComponentModel.DataAnnotations;

namespace WoW.Model
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        [Required]
        public Rank Rank { get; set; }

        public Member() { }

        public Member(int _memberId, string _name, Rank _rank)
        {
            MemberId = _memberId;
            Name = _name;
            Rank = _rank;
        }

        public override string ToString()
        {
            return $"Member: ID = {MemberId}, Name = {Name}, Rank = {Rank}";
        }
    }
}
