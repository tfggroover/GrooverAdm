using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Spotify
{
    public class AuthorizationCodeFlowResponse : IAuthResponse
    {

        public string Refresh_Token { get; set; }
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public int Expires_in { get; set; }
        public string Scope { get; set; }
    }

    public class RefreshTokenResponse : IAuthResponse
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public int Expires_in { get; set; }
        public string Scope { get; set; }
        public string Refresh_Token { get => ""; set => Refresh_Token = ""; }
    }

    public interface IAuthResponse
    {
        string Access_token { get; set; }
        string Token_type { get; set; }
        int Expires_in { get; set; }
        string Scope { get; set; }
        string Refresh_Token { get; set; }
    }

}
