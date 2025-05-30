using System;

namespace WowGMSBackend.Model
{
    /// <summary>
    /// Represents a boss entity with a display name and a unique slug identifier.
    /// </summary>
    public class Boss
    {
        public string DisplayName { get; set; }
        public string Slug { get; set; }
    }
}
