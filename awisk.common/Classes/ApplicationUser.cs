using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace awisk.common.Classes
{
    public partial class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(250, ErrorMessage = "Must be between 2 and 250 characters", MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Gender { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public Guid CreatedBy { get; set; } = Guid.Empty;
        public bool IsVerified { get; set; }
        public string? ProfileImageUrl { get; set; } = string.Empty;
    }
}
