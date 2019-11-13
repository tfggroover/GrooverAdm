using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using GrooverAdmSPA.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GrooverAdmSPA.Controllers
{
    [Route("[Controller]")]
    public class HomeController : Controller
    {

        private readonly FirebaseApp firebaseApp;
        private readonly IConfiguration Configuration;
        private readonly FirestoreDb firestoreDb;

        public HomeController(IConfiguration configuration, FirebaseApp app, FirestoreDb db)
        {
            Configuration = configuration;
            firebaseApp = app;
            firestoreDb = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Redirect")]
        public async Task<IActionResult> Auth(string refresh_token = null)
        {
            if (string.IsNullOrWhiteSpace(refresh_token))
                return AuthorizationCodeFlow();
            else
                return await RefreshTokenFlow(refresh_token);
        }


        [HttpGet("callback")]
        public async Task<IActionResult> AuthCallback(string code)
        {
            if (string.IsNullOrEmpty(Request.Cookies["State"]))
            {
                return BadRequest("State cookie not set or expired. Maybe you took too long to authorize. Please try again.");
            }
            else if(Request.Cookies["State"] != Request.Query["State"])
            {
                return BadRequest("State verification failed.");
            }

            object result = null;

            if (code.Length > 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    var spotiCredentials = await AuthRequest(code, client);

                    var token = await RequestUserDataAndGenerateToken(client, spotiCredentials);

                    result = new
                    {
                        Spotify = spotiCredentials,
                        Firebase = token
                    };

                }
            }

            return Json(result);
        }


        private async Task<IActionResult> RefreshTokenFlow(string refresh_token)
        {
            object result = null;
            using (HttpClient client = new HttpClient())
            {
                var spotiCredentials = await RefreshAuthRequest(refresh_token, client);

                var token = await RequestUserDataAndGenerateToken(client, spotiCredentials);

                result = new
                {
                    Spotify = spotiCredentials,
                    Firebase = token
                };
            }

            return Json(result);
        }

        private IActionResult AuthorizationCodeFlow()
        {
            var cookieNonce = Request.Cookies["State"];
            string nonce;
            if (string.IsNullOrEmpty(cookieNonce))
            {
                nonce = Guid.NewGuid().ToString("N");
            }
            else
                nonce = cookieNonce;
            //string.IsNullOrEmpty(cookieNonce) ? RandomNumberGenerator.Create().GetBytes(byteNonce) :  Encoding.UTF32.GetBytes(cookieNonce.ToCharArray(), byteNonce);

            var secure = Request.Host.Host == "localhost";

            Response.Cookies.Append("State", nonce, new Microsoft.AspNetCore.Http.CookieOptions
            {
                MaxAge = new TimeSpan(30, 0, 0, 0),
                Secure = secure,
                HttpOnly = true
            });

            var redirectUri = Configuration["RedirectURI"];
            var clientID = Configuration["ClientID"];
            var authEndpoint = Configuration["AuthEndpoint"];


            var spotifyCall = $"{authEndpoint}?client_id={UrlEncoder.Default.Encode(clientID)}&response_type=code&redirect_uri={UrlEncoder.Default.Encode(redirectUri)}&state={UrlEncoder.Default.Encode(nonce)}&scope={UrlEncoder.Default.Encode("user-read-private user-read-email")}";

            return Redirect(spotifyCall);
        }

        private async Task<string> RequestUserDataAndGenerateToken(HttpClient client, SpotifyAuthResponse spotiCredentials)
        {
            var userData = await UserInfoRequest(client, spotiCredentials);

            var auth = FirebaseAuth.GetAuth(firebaseApp);

            try
            {
                await auth.GetUserAsync(userData.Id); // If the user does not exist an exception is thrown
                await auth.UpdateUserAsync(new UserRecordArgs()
                {
                    Email = userData.Email,
                    DisplayName = userData.Display_name,
                    Uid = userData.Id,
                    PhotoUrl = userData.Images.FirstOrDefault()
                });
            }
            catch (FirebaseAuthException)
            {
                await auth.CreateUserAsync(new UserRecordArgs()
                {
                    Email = userData.Email,
                    DisplayName = userData.Display_name,
                    Uid = userData.Id,
                    PhotoUrl = userData.Images.FirstOrDefault()
                });
            }

            var token = await auth.CreateCustomTokenAsync(userData.Id);
            return token;
        }

        private static async Task<SpotifyUserInfo> UserInfoRequest(HttpClient client, SpotifyAuthResponse spotiCredentials)
        {
            var userDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.spotify.com/v1/me"),
                Method = HttpMethod.Get
            };

            userDataRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", spotiCredentials.Access_token);

            var userDataResponse = await client.SendAsync(userDataRequest);

            var userData = JsonConvert.DeserializeObject<SpotifyUserInfo>(await userDataResponse.Content.ReadAsStringAsync());
            return userData;
        }


        private async Task<SpotifyRefreshTokenResponse> RefreshAuthRequest(string refresh_token, HttpClient client)
        {
            var authRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{Configuration["AccessEndpoint"]}"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new[]
                {
                            new KeyValuePair<string, string>("refresh_token", refresh_token),
                            new KeyValuePair<string, string>("grant_type", "refresh_token")
                        })
            };

            authRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(Configuration["ClientID"] + ":" + Configuration["ClientSecret"])));

            var response = await client.SendAsync(authRequest);

            var responseContent = await response.Content.ReadAsStringAsync();

            var spotiCredentials = JsonConvert.DeserializeObject<SpotifyRefreshTokenResponse>(responseContent);
            return spotiCredentials;
        }

        private async Task<SpotifyAuthorizationCodeFlowResponse> AuthRequest(string code, HttpClient client)
        {
            var authRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{Configuration["AccessEndpoint"]}"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new[]
                {
                            new KeyValuePair<string, string>("code", code),
                            new KeyValuePair<string, string>("redirect_uri", Configuration["RedirectURI"]),
                            new KeyValuePair<string, string>("grant_type", "authorization_code")
                        })
            };

            authRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(Configuration["ClientID"] + ":" + Configuration["ClientSecret"])));

            var response = await client.SendAsync(authRequest);

            var responseContent = await response.Content.ReadAsStringAsync();

            var spotiCredentials = JsonConvert.DeserializeObject<SpotifyAuthorizationCodeFlowResponse>(responseContent);
            return spotiCredentials;
        }

        public IActionResult AuthResponse(string access_token, string token_type, string expires_in, string state)
        {
            throw new NotImplementedException();
        }
    }
}