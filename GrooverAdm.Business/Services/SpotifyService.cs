using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GrooverAdm.Entities.Spotify;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GrooverAdm.Business.Services
{
    public class SpotifyService
    {
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private readonly string _accessEndpoint;
        private readonly string _redirectURI;
        private readonly string _clientSecret;
        private readonly string _scopes;

        public SpotifyService(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration["ClientId"];
            _accessEndpoint = _configuration["AccessEndpoint"];
            _redirectURI = _configuration["RedirectURI"];
            _clientSecret = _configuration["ClientSecret"];
            _scopes = _configuration["Scopes"];
        }


        public async Task<AuthorizationCodeFlowResponse> AuthRequest(string code, HttpClient client)
        {
            var authRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_accessEndpoint}"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new[]
                {
                            new KeyValuePair<string, string>("code", code),
                            new KeyValuePair<string, string>("redirect_uri", _redirectURI),
                            new KeyValuePair<string, string>("grant_type", "authorization_code")
                        })
            };

            authRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(_clientId + ":" + _clientSecret)));

            var response = await client.SendAsync(authRequest);

            var responseContent = await response.Content.ReadAsStringAsync();

            var spotiCredentials = JsonConvert.DeserializeObject<AuthorizationCodeFlowResponse>(responseContent);
            return spotiCredentials;
        }

        public async Task<RefreshTokenResponse> RefreshAuthRequest(string refresh_token, HttpClient client)
        {
            var authRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_accessEndpoint}"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new[]
                {
                            new KeyValuePair<string, string>("refresh_token", refresh_token),
                            new KeyValuePair<string, string>("grant_type", "refresh_token")
                        })
            };

            authRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(_clientId + ":" + _clientSecret)));

            var response = await client.SendAsync(authRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var spotiCredentials = JsonConvert.DeserializeObject<RefreshTokenResponse>(responseContent);
                return spotiCredentials;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserInfo> UserInfoRequest(HttpClient client, AuthResponse spotiCredentials)
        {
            var userDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.spotify.com/v1/me"),
                Method = HttpMethod.Get
            };

            userDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", spotiCredentials.Access_token);

            var userDataResponse = await client.SendAsync(userDataRequest);

            var userData = JsonConvert.DeserializeObject<UserInfo>(await userDataResponse.Content.ReadAsStringAsync());
            return userData;
        }

        public async Task<GetPlaylistsResponse> GetMyPlaylists(HttpClient client, string accessToken)
        {
            var playlistDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.spotify.com/v1/me/playlists?limit=25"),
                Method = HttpMethod.Get
            };

            playlistDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var playlistDataResponse = await client.SendAsync(playlistDataRequest);
            if (playlistDataResponse.IsSuccessStatusCode)
            {
                var playlistData = JsonConvert.DeserializeObject<GetPlaylistsResponse>(await playlistDataResponse.Content.ReadAsStringAsync());
                return playlistData;
            }

            return null;
        }

        public async Task<GetSongsResponse> GetSongsFromPlaylist(HttpClient client, string accessToken, string playlistId)
        {

            var songsDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{ playlistId }/tracks?fields=items.track(name%2Cid%2Cartists(name))"),
                Method = HttpMethod.Get
            };

            songsDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var songsDataResponse = await client.SendAsync(songsDataRequest);
            if (songsDataResponse.IsSuccessStatusCode)
            {
                var songsData = JsonConvert.DeserializeObject<GetSongsResponse>(await songsDataResponse.Content.ReadAsStringAsync());
                while (songsData.Next != null)
                {
                    var newRequest = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(songsData.Next),
                        Method = HttpMethod.Get
                    };
                    newRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await client.SendAsync(newRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = JsonConvert.DeserializeObject<GetSongsResponse>(await response.Content.ReadAsStringAsync());
                        songsData.Next = responseData.Next;
                        songsData.Items.AddRange(responseData.Items);
                    }
                }
                return songsData;
            }

            return null;
        }


        public async Task<List<Artist>> GetArtists(HttpClient client, string accessToken, string artistsIds)
        {

            var artistsDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.spotify.com/v1/artists/?ids=" + artistsIds),
                Method = HttpMethod.Get
            };

            artistsDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var artistsDataResponse = await client.SendAsync(artistsDataRequest);
            if (artistsDataResponse.IsSuccessStatusCode)
            {
                var artistsData = JsonConvert.DeserializeObject<List<Artist>>(await artistsDataResponse.Content.ReadAsStringAsync());
                return artistsData;
            }

            return null;
        }

        public async Task<GetTopSongsResponse> GetUsersTopTracks(HttpClient client, string accessToken)
        {
            var songsDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.spotify.com/v1/me/top/tracks?limit=50"),
                Method = HttpMethod.Get
            };

            songsDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var songsDataResponse = await client.SendAsync(songsDataRequest);
            if (songsDataResponse.IsSuccessStatusCode)
            {
                var songsData = JsonConvert.DeserializeObject<GetTopSongsResponse>(await songsDataResponse.Content.ReadAsStringAsync());
                return songsData;
            }

            return null;
        }


        public async Task<GetSongsResponse> GetPlaylist(HttpClient client, string accessToken, string playlistId)
        {

            var songsDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{ playlistId }?fields=items.track(name%2Cid%2Cartists(name))"),
                Method = HttpMethod.Get
            };

            songsDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var songsDataResponse = await client.SendAsync(songsDataRequest);
            if (songsDataResponse.IsSuccessStatusCode)
            {
                var songsData = JsonConvert.DeserializeObject<GetSongsResponse>(await songsDataResponse.Content.ReadAsStringAsync());
                while (songsData.Next != null)
                {
                    var newRequest = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(songsData.Next),
                        Method = HttpMethod.Get
                    };
                    newRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await client.SendAsync(newRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = JsonConvert.DeserializeObject<GetSongsResponse>(await response.Content.ReadAsStringAsync());
                        songsData.Next = responseData.Next;
                        songsData.Items.AddRange(responseData.Items);
                    }
                }
                return songsData;
            }

            return null;
        }
    }
}
