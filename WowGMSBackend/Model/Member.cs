using System.ComponentModel.DataAnnotations;

namespace WowGMSBackend.Model
{
    // Represents a member in the system (e.g., guild member)
    public class Member
    {
        // Primary key for Member entity
        [Key]
        public int MemberId { get; set; }

        // Member's name; required and limited to 20 characters
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        // Member's password; required field, stored as plain text (should be hashed in production)
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Member's rank; required field
        [Required]
        public Rank Rank { get; set; }

        // Navigation property: list of characters associated with this member
        public List<Character> Characters { get; set; } = new();

        // Parameterless constructor required by EF
        public Member() { }

        // Constructor for manual instantiation
        public Member(int memberId, string name, string password, Rank rank)
        {
            MemberId = memberId;
            Name = name;
            Password = password;
            Rank = rank;
        }

        // String representation of the Member entity
        public override string ToString()
        {
            return $"Member: ID = {MemberId}, Name = {Name}, Rank = {Rank}";
        }
    }
}
