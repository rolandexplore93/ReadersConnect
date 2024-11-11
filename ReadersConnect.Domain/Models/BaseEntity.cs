using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Domain.Models
{
    public class BaseEntity
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow.AddHours(1);
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow.AddHours(1);
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        [MaxLength(100)]
        public string? DeletedBy { get; set; }
    }
}
