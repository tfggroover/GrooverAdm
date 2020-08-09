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
using GrooverAdm.Business.Services;
using GrooverAdm.Entities.Application;
using GrooverAdm.Entities.Spotify;
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
        private readonly SpotifyService _spotifyService;

        public HomeController(IConfiguration configuration, FirebaseApp app, FirestoreDb db, SpotifyService spotifyService)
        {
            Configuration = configuration;
            firebaseApp = app;
            firestoreDb = db;
            _spotifyService = spotifyService;
        }

        /// <summary>
        /// NO LOS HE PROBADO EN LA NUEVA VERSIÓN, PERO HA SIDO COPIAR Y PEGAR, DEBERÍA IR
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        [HttpGet("Auth")]
        public async Task<IActionResult> Auth(string refresh_token = null)
        {
            if (string.IsNullOrWhiteSpace(refresh_token))
                return AuthorizationCodeFlow();
            else
                return await RefreshTokenFlow(refresh_token);
        }

        /// <summary>
        /// IGUAL QUE ARRIBA
        /// </summary>
        /// <param name="code"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        [HttpGet("callback")]
        public async Task<IActionResult> AuthCallback(string code, string nonce = null)
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
                    var spotiCredentials = await _spotifyService.AuthRequest(code, client);

                    var userData = await _spotifyService.UserInfoRequest(client, spotiCredentials);

                    var token = await GenerateToken(userData, spotiCredentials);

                    result = new
                    {
                        Spotify = spotiCredentials,
                        SpotifyUserData = userData,
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
                var spotiCredentials = await _spotifyService.RefreshAuthRequest(refresh_token, client);

                if(spotiCredentials == null)
                {
                    return AuthorizationCodeFlow();
                }
                var userData = await _spotifyService.UserInfoRequest(client, spotiCredentials);

                var token = await GenerateToken(userData, spotiCredentials);

                result = new
                {
                    Spotify = spotiCredentials,
                    SpotifyUserData = userData,
                    Firebase = token
                };
            }

            return Json(result);
        }

        private IActionResult AuthorizationCodeFlow()
        {

            var  nonce = Guid.NewGuid().ToString("N");

            //string.IsNullOrEmpty(cookieNonce) ? RandomNumberGenerator.Create().GetBytes(byteNonce) :  Encoding.UTF32.GetBytes(cookieNonce.ToCharArray(), byteNonce);

            var secure = Request.Host.Host == "localhost";

            Response.Cookies.Append("State", nonce, new Microsoft.AspNetCore.Http.CookieOptions
            {
                MaxAge = new TimeSpan(30, 0, 0, 0),
                Secure = secure,
                Domain = HttpContext.Request.Host.Host,
                HttpOnly = true
            });

            var redirectUri = Configuration["RedirectURI"];
            var clientID = Configuration["ClientID"];
            var authEndpoint = Configuration["AuthEndpoint"];
            var scopes = Configuration["Scopes"];

            Console.Error.WriteLine($"uri ={redirectUri} ; id = {clientID} ; endpoint = {authEndpoint} ; scopes = {scopes}");


            var spotifyCall = $"{authEndpoint}?client_id={UrlEncoder.Default.Encode(clientID)}&response_type=code&redirect_uri={UrlEncoder.Default.Encode(redirectUri)}&state={UrlEncoder.Default.Encode(nonce)}&scope={UrlEncoder.Default.Encode(scopes)}";

            return Redirect(spotifyCall);
        }


        private async Task<string> GenerateToken(UserInfo userData, AuthResponse credentials)
        {

            var auth = FirebaseAuth.GetAuth(firebaseApp);

            try
            {
                await auth.GetUserAsync(userData.Id); // If the user does not exist an exception is thrown
                await auth.UpdateUserAsync(new UserRecordArgs()
                {
                    Email = userData.Email,
                    DisplayName = userData.Display_name,
                    Uid = userData.Id,
                    PhotoUrl = userData.Images.FirstOrDefault()?.Url
                });
            }
            catch (FirebaseAuthException)
            {
                await auth.CreateUserAsync(new UserRecordArgs()
                {
                    Email = userData.Email,
                    DisplayName = userData.Display_name,
                    Uid = userData.Id,
                    PhotoUrl = userData.Images.FirstOrDefault()?.Url
                });
            }
           // var user = new User(userData, credentials.Access_token, credentials.Expires_in, DateTime.UtcNow);
            var reference = firestoreDb.Collection("users").Document($"{userData.Id}");
            //if ((await reference.GetSnapshotAsync()).Exists)
            //    await reference.UpdateAsync(user.ToDictionary());
            //else
            //    await reference.CreateAsync(user.ToDictionary());

            var token = await auth.CreateCustomTokenAsync(userData.Id);
            return token;
        }

    }
}