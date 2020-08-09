using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Spotify
{
    public class AuthorizationCodeFlowResponse : AuthResponse
    {
        public string Refresh_token { get; set; }
    }

    public class RefreshTokenResponse : AuthResponse
    {
    }

    public class AuthResponse
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public int Expires_in { get; set; }
        public string Scope { get; set; }
    }

}
