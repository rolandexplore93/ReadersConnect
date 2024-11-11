using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Web.Dtos
{
    public class CreateRoleRequestDto
    {
        [Required]
        public string RoleName { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
