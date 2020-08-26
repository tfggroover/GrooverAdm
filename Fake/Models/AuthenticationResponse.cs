using GrooverAdm.Entities.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fake.Models
{
    public class AuthenticationResponse
    {
        public IAuthResponse Spotify { get; set; }
        public UserInfo SpotifyUserData { get; set; }
        public string Firebase { get; set; }

    }
}
