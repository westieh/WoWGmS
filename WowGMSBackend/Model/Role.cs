namespace WowGMSBackend.Model
{
    // Defines the combat role of a character in the raid group
    public enum Role
    {
        Tank,       // Absorbs damage and protects the group
        Healer,     // Restores health to group members
        MeleeDPS,   // Deals damage at close range
        RangedDPS   // Deals damage from a distance
    }
}
