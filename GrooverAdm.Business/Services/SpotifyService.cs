using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
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
            var success = false;
            while (!success)
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
                if (response.IsSuccessStatusCode)
                {

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var spotiCredentials = JsonConvert.DeserializeObject<AuthorizationCodeFlowResponse>(responseContent);
                    return spotiCredentials;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)response.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }

        public async Task<RefreshTokenResponse> RefreshAuthRequest(string refresh_token, HttpClient client)
        {
            var success = false;
            while (!success)
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
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)response.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }

        public async Task<UserInfo> UserInfoRequest(HttpClient client, AuthResponse spotiCredentials)
        {
            var success = false;
            while (!success)
            {

                var userDataRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://api.spotify.com/v1/me"),
                    Method = HttpMethod.Get
                };

                userDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", spotiCredentials.Access_token);

                var userDataResponse = await client.SendAsync(userDataRequest);
                if (userDataResponse.IsSuccessStatusCode)
                {
                    var userData = JsonConvert.DeserializeObject<UserInfo>(await userDataResponse.Content.ReadAsStringAsync());
                    return userData;
                }
                else if (userDataResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)userDataResponse.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }

        public async Task<GetPlaylistsResponse> GetMyPlaylists(HttpClient client, string accessToken)
        {
            var success = false;
            while (!success)
            {
                var playlistDataRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://api.spotify.com/v1/me/playlists?limit=25"),
                    Method = HttpMethod.Get
                };

                playlistDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(playlistDataRequest);
                if (response.IsSuccessStatusCode)
                {
                    var playlistData = JsonConvert.DeserializeObject<GetPlaylistsResponse>(await response.Content.ReadAsStringAsync());
                    return playlistData;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)response.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }

        public async Task<GetSongsResponse> GetSongsFromPlaylist(HttpClient client, string accessToken, string playlistId)
        {
            var success = false;

            while (!success)
            {
                var songsDataRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{ playlistId }/tracks?fields=items.track(name%2Cid%2Cartists(id%2Cname))"),
                    Method = HttpMethod.Get
                };

                songsDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(songsDataRequest);
                if (response.IsSuccessStatusCode)
                {
                    var songsData = JsonConvert.DeserializeObject<GetSongsResponse>(await response.Content.ReadAsStringAsync());
                    while (songsData.Next != null)
                    {
                        var newRequest = new HttpRequestMessage()
                        {
                            RequestUri = new Uri(songsData.Next),
                            Method = HttpMethod.Get
                        };
                        newRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                        var innerResponse = await client.SendAsync(newRequest);
                        if (innerResponse.IsSuccessStatusCode)
                        {
                            var responseData = JsonConvert.DeserializeObject<GetSongsResponse>(await innerResponse.Content.ReadAsStringAsync());
                            songsData.Next = responseData.Next;
                            songsData.Items.AddRange(responseData.Items);
                        }
                        else if (innerResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                        {
                            Thread.Sleep((int)innerResponse.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                        }
                    }
                    return songsData;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)response.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }


        public async Task<List<Artist>> GetArtists(HttpClient client, string accessToken, string artistsIds)
        {

            var success = false;
            while (!success)
            {
                var artistsDataRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://api.spotify.com/v1/artists/?ids=" + artistsIds),
                    Method = HttpMethod.Get
                };

                artistsDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(artistsDataRequest);
                if (response.IsSuccessStatusCode)
                {
                    var artistsData = JsonConvert.DeserializeObject<ArtistResponse>(await response.Content.ReadAsStringAsync());
                    return artistsData.Artists;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)response.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }

        public async Task<GetTopSongsResponse> GetUsersTopTracks(HttpClient client, string accessToken)
        {
            var success = false;
            while (!success)
            {
                var songsDataRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://api.spotify.com/v1/me/top/tracks?limit=50"),
                    Method = HttpMethod.Get
                };

                songsDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.SendAsync(songsDataRequest);
                if (response.IsSuccessStatusCode)
                {
                    var songsData = JsonConvert.DeserializeObject<GetTopSongsResponse>(await response.Content.ReadAsStringAsync());
                    return songsData;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)response.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }


        public async Task<Entities.Spotify.Playlist> GetPlaylist(HttpClient client, string accessToken, string playlistId)
        {
            var success = false;
            while (!success)
            {
                var playlistRequest = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{ playlistId }"),
                    Method = HttpMethod.Get
                };

                playlistRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var playlistResponse = await client.SendAsync(playlistRequest);

                if (playlistResponse.IsSuccessStatusCode)
                {
                    var playlist = JsonConvert.DeserializeObject<Entities.Spotify.Playlist>(await playlistResponse.Content.ReadAsStringAsync());
                    while (playlist.Tracks.Next != null)
                    {
                        var newRequest = new HttpRequestMessage()
                        {
                            RequestUri = new Uri(playlist.Tracks.Next),
                            Method = HttpMethod.Get
                        };
                        newRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                        var response = await client.SendAsync(newRequest);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = JsonConvert.DeserializeObject<GetSongsResponse>(await response.Content.ReadAsStringAsync());
                            playlist.Tracks.Next = responseData.Next;
                            playlist.Tracks.Items.AddRange(responseData.Items);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                        {
                            Thread.Sleep((int)response.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                        }
                    }
                    return playlist;
                }
                else if (playlistResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    Thread.Sleep((int)playlistResponse.Headers.RetryAfter.Delta.Value.TotalMilliseconds);
                else
                    break;
            }
            return null;
        }
    }
}
