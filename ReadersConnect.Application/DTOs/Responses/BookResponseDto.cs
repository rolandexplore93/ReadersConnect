using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Responses
{
    public class BookResponseDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public List<string> Genre { get; set; }
        public DateTime PublishedDate { get; set; }
        public string AvailabilityStatus { get; set; }
        public int Copies { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
    }
}