using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ReadersConnect.Application.DTOs.Requests
{
    public class BookRequestDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [Required]
        public DateTime ReturnedDate { get; set; }
    }
}