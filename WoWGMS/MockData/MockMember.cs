using WoW.Model;

namespace WoWGMS.MockData
{
    public class MockMember
    {
        public static List<Member> GetMockMembers()
        {
            return new List<Member>
            {
                new Member(1, "Lars Larsen", Rank.Trialist),
                new Member(2, "Simon Simonsen", Rank.Raider),
                new Member(3, "Søren Sørensen", Rank.Officer)
            };
        }
    }
}