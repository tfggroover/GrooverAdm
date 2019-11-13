using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public class SpotifyAuthorizationCodeFlowResponse : SpotifyAuthResponse
    {
        public string Refresh_token { get; set; }
    }

    public class SpotifyRefreshTokenResponse : SpotifyAuthResponse
    {
    }

    public class SpotifyAuthResponse
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public int Expires_in { get; set; }
        public string Scope { get; set; }
    }

}
