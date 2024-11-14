using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Domain.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public List<string> Genre { get; set; }
        public DateTime PublishedDate { get; set; }
        public string AvailabilityStatus { get; set; } = "Available";
        public int Copies { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
    }
}
