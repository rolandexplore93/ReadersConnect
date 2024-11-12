using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Application.Helpers.Configuration
{
    public class ConfigurationManager
    {
    }

    public class JwtConfig
    {
        public const string Position = "Jwt";
        public string SwaggerKey { get; set; }
    }
}
