using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Dtos
{
    public class CreatePermissionDto
    {
        [Required]
        public string PermissionName { get; set; }
    }
}
