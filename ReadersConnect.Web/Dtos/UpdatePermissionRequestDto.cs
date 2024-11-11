using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Web.Dtos
{
    public class UpdatePermissionRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string PermissionName { get; set; }
    }
}
