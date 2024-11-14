using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Requests
{
    public class AssignRoleRequestDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
