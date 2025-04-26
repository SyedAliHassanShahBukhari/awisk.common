using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.Classes
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public bool ShowOpenApiDocs { get; set; }
        public string AllowedOrigins { get; set; } = string.Empty;
    }
}
