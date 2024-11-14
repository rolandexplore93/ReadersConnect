using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.DTOs.Requests
{
    public class ApproveOrRejectBookRequestDTO
    {
        public int RequestId { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }

    }
}
