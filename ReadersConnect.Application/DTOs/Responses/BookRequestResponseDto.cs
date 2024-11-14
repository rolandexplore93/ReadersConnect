using ReadersConnect.Domain.Models.Identity;
using ReadersConnect.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Responses
{
    public class BookRequestResponseDto
    {
        public int RequestId { get; set; }
        public int BookId { get; set; }
        public BookResponseDto Book {  get; set; }
        public string ApplicationUserId { get; set; }
        public UserResponseDto ApplicationUser { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string Status { get; set; }
    }
}
