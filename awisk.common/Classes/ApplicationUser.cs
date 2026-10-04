using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace awisk.common.Classes
{
    public partial class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(250, ErrorMessage = "Must be between 2 and 250 characters", MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Gender as the consuming solution's enum value (1 = Male, 2 = Female), not free text.
        /// Was nvarchar(50) holding "Male"/"Female" until 3.2.0, which left the same fact stored two
        /// ways in one database -- as text here and as an int on the domain tables that report on it.
        /// Nullable because a value that cannot be mapped is unknown, not male.
        /// </summary>
        public int? Gender { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Who created this user, as text, matching CreatedBy on every other audited table.
        /// Was a uniqueidentifier until 3.2.0, and the one table in the schema that disagreed; the
        /// default of Guid.Empty also meant "unknown" was stored as a value rather than as NULL.
        /// </summary>
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        public bool IsVerified { get; set; }
        public string? ProfileImageUrl { get; set; } = string.Empty;
    }
}
