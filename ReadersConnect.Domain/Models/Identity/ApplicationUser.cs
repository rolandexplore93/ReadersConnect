using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ReadersConnect.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Gender { get; set; }
        public bool IsActive { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostCode { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
        public bool IsPasswordExpire { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        [MaxLength(100)]
        public string? DeletedBy { get; set; }
    }
}
