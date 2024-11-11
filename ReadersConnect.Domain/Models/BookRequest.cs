using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Domain.Models
{
    public class BookRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        public int BookId { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime ReturnedDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
