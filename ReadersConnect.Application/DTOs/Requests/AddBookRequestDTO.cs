using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Requests
{
    public class AddBookRequestDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public List<string> Genre { get; set; }
        [Required]
        public DateTime PublishedDate { get; set; }
        [Required]
        public int Copies { get; set; }
        public string? ImageUrl { get; set; }
    }
}
