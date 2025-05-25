using WowGMSBackend.Model;

namespace WowGMSBackend.MockData
{
    public class MockMember
    {
        public static List<Member> GetMockMembers()
        {
            return new List<Member>
            {
                new Member(1, "Lars Larsen", "1234", Rank.Trialist),
                new Member(2, "Simon Simonsen","4321", Rank.Raider),
                new Member(3, "Søren Sørensen","hey", Rank.Officer)
            };
        }
    }
}