﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowGMSBackend.Model
{
    /// <summary>
    /// Represents a guild application submitted by a player, including character details and boss kill history.
    /// </summary>
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }

        [Required]
        [MaxLength(12)]
        public string? CharacterName { get; set; }

        [Required]
        [MaxLength(30)]
        public string? DiscordName { get; set; }

        [Required]
        public string? Password { get; set; }

        public Class Class { get; set; }
        public Role Role { get; set; }
        public ServerName ServerName { get; set; }

        public DateTime SubmissionDate { get; set; }

        public Member? ProcessedBy { get; set; }

        public string? Note { get; set; }

        public List<BossKill> BossKills { get; set; } = new();

        public bool Approved { get; set; }
    }
}
