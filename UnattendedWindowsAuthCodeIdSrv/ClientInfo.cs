using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnattendedWindowsAuthCodeIdSrv
{
    public class ClientInfo
    {
        public string Id { get; set; }

        public string Secret { get; set; }

        public string RedirectUri { get; set; }

        public string AuthorityUri { get; set; }

        public string Scopes { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
