using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Domain.Models
{
    public class BorrowingRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }
        public int RequestId { get; set; }
        public DateTime ReturnedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; } = DateTime.UtcNow;
        public bool IsBookReturned { get; set; } = false;
        public string? BookReturnedConfirmedBy { get; set; }
        public DateTime ActualReturnedDate { get; set; }
    }
}
