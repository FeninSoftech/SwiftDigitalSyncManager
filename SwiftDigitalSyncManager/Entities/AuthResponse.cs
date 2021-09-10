using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Entities
{
    public class AuthResponse
    {
        public Apitoken apiToken { get; set; }
    }

    public class Apitoken
    {
        public string consumer_key { get; set; }
        public string oauth_token { get; set; }
        public string oauth_token_secret { get; set; }
        public string ClientSecret { get; set; }
    }
}
