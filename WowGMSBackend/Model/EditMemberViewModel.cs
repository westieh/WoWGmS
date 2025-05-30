using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowGMSBackend.Model
{
    // ViewModel for editing a member's details
    public class EditMemberViewModel
    {
        // Unique identifier for the member
        public int MemberId { get; set; }

        // Name of the member; required and limited to 20 characters
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        // Rank of the member; required field
        [Required]
        public Rank Rank { get; set; }
    }
}
