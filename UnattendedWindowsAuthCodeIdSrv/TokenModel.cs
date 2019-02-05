using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UnattendedWindowsAuthCodeIdSrv
{
    class TokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresInSeconds { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
