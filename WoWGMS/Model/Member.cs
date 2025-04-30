namespace WoW.Model
{
    public class Member
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public Rank Rank { get; set; }

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
