using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.Classes
{
    public class AuthConfig
    {
        public string AuthScheme { get; set; } = string.Empty;
        public string AuthCookie { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = string.Empty;
        public string LogoutUrl { get; set; } = string.Empty;
        public string AccessDeniedUrl { get; set; } = string.Empty;
        public int TokenExpire { get; set; } = 30;
    }
}
