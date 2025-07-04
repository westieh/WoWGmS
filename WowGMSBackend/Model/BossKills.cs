﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WowGMSBackend.Model
{
    /// <summary>
    /// Represents the number of kills a character or application has for a specific boss.
    /// </summary>
    public class BossKill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string BossSlug { get; set; }

        [Required]
        public int KillCount { get; set; }

        [ForeignKey("Character")]
        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        [ForeignKey("Application")]
        public int? ApplicationId { get; set; }
        public Application? Application { get; set; }
    }
}
