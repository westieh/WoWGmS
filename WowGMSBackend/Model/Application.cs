namespace WoW.Model
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public string CharacterName { get; set; }
        public string DiscordName { get; set; }
        public Class Class { get; set; }
        public Role Role { get; set; }
        public DateTime SubmissionDate { get; set; }
        public Member ProcessedBy { get; set; }
        public string Note { get; set; }
        public bool Approved { get; set; }
    }
}
