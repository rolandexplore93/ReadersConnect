﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Requests
{
    public class CreatePermissionRequestDto
    {
        [Required]
        public string PermissionName { get; set; }
    }
}
