namespace WoW.Model
{
    public class Character
    {
        public string CharacterName { get; set; }
        public string RealmName { get; set; }
        public Class Class { get; set; }
        public Role Role { get; set; }

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
    }
}
