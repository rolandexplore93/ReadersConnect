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
        public string SwaggerKey { get; set; } = "123456";
    }

    public class JwtSettings
    {
        public string? Site { get; set; }
        public string? SigningKey { get; set; }
        public string? ExpiryInMinutes { get; set; }
        public string? Issuer { get; set; }
    }
}
