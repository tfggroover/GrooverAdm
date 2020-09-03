using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GrooverAdm.Models;
using GrooverAdm.Pages;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using GrooverAdm.Business.Services;
using GrooverAdm.Business.Services.User;
using GrooverAdm.Entities.Application;
using GrooverAdm.Entities.Spotify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace GrooverAdmSPA.Controllers
{
    /// <summary>
    /// Will be renamed to authController
    /// </summary>
    [Route("[Controller]")]
    public class HomeController : Controller
    {

        private readonly IConfiguration Configuration;
        private readonly SpotifyService _spotifyService;
        private readonly IUserService userService;
        private readonly ILogger log;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="spotifyService"></param>
        /// <param name="userService"></param>
        public HomeController(IConfiguration configuration, SpotifyService spotifyService, IUserService userService, ILogger<HomeController> log)
        {
            Configuration = configuration;
            _spotifyService = spotifyService;
            this.userService = userService;
            this.log = log;
        }


        /// <summary>
        /// Autenticación para el móvil y la App
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        /// <response code="400">The specified refresh token is not valid</response>
        [HttpGet("Auth")]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, Type = typeof(AuthenticationResponse))]
        public async Task<ActionResult<AuthenticationResponse>> Auth(string refresh_token)
        {
            if(string.IsNullOrEmpty(refresh_token))
                return AuthorizationCodeFlow();
            return await RefreshTokenFlow(refresh_token);
        }

        /// <summary>
        /// Callback de spotify con un token fresco
        /// </summary>
        /// <param name="code">Token</param>
        /// <param name="State">Cookie de estado</param>
        /// <returns></returns>
        [HttpGet("callback")]
        public async Task<ActionResult<AuthenticationResponse>> AuthCallback(string code, string State = null)
        {
            this.log.LogInformation("Entered mobile callback");
            if (string.IsNullOrEmpty(Request.Cookies["State"]))
            {
                return BadRequest("State cookie not set or expired. Maybe you took too long to authorize. Please try again.");
            }
            else if (Request.Cookies["State"] != Request.Query["State"])
            {
                return BadRequest("State verification failed.");
            }

            AuthenticationResponse result = null;

            if (code.Length > 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    log.LogInformation("Entering Spotify AuthResquest");
                    var spotiCredentials = await _spotifyService.AuthRequest(code, client, false);

                    if (spotiCredentials == null)
                        return BadRequest("Spotify returned a 400");
                    
                    log.LogInformation($"Spotify credentials provided: {JsonConvert.SerializeObject(spotiCredentials)}");
                    var userData = await _spotifyService.UserInfoRequest(client, spotiCredentials);

                    log.LogInformation($"User data: name: {userData.Display_name} user: {userData.Id}");
                    var token = await this.userService.GenerateToken(userData, spotiCredentials);

                    result = new AuthenticationResponse
                    {
                        Spotify = spotiCredentials,
                        SpotifyUserData = userData,
                        Firebase = token
                    };

                    log.LogInformation($"Returned answer {JsonConvert.SerializeObject(result)}");

                }
            }

            return Json(result);
        }


        /// <summary>
        /// Callback de spotify con un token fresco
        /// </summary>
        /// <param name="code">Token</param>
        /// <param name="State">Cookie de estado</param>
        /// <returns></returns>
        [HttpGet("web-callback")]
        public async Task<IActionResult> AuthWebCallback(string code, string State = null)
        {

            if (string.IsNullOrEmpty(Request.Cookies["State"]))
            {
                return BadRequest("State cookie not set or expired. Maybe you took too long to authorize. Please try again.");
            }
            else if (Request.Cookies["State"] != Request.Query["State"])
            {
                return BadRequest("State verification failed.");
            }

            var model = new AuthPopupModel
            {
                ApiKey = Configuration["ApiKey"],
                DbUrl = Configuration["DbUrl"]
            };

            if (code.Length > 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    var spotiCredentials = await _spotifyService.AuthRequest(code, client, true);
                    if (spotiCredentials == null)
                        BadRequest("Invalid request to spotify");
                    var userData = await _spotifyService.UserInfoRequest(client, spotiCredentials);

                    var token = await this.userService.GenerateToken(userData, spotiCredentials);

                    model.DisplayName = userData.Display_name;
                    model.PhotoUrl = userData.Images[0]?.Url;
                    model.Token = token;
                    model.SpotiToken = spotiCredentials.Access_token;
                    model.RefreshToken = spotiCredentials.Refresh_Token;
                }
            }

            return View("AuthPopup", model);
        }


        private async Task<ActionResult<AuthenticationResponse>> RefreshTokenFlow(string refresh_token)
        {
            AuthenticationResponse result = null;
            using (HttpClient client = new HttpClient())
            {
                var spotiCredentials = await _spotifyService.RefreshAuthRequest(refresh_token, client);

                if (spotiCredentials == null)
                {
                    return BadRequest("The refresh token provided is not valid");
                }
                var userData = await _spotifyService.UserInfoRequest(client, spotiCredentials);

                var token = await this.userService.GenerateToken(userData, spotiCredentials);

                result = new AuthenticationResponse
                {
                    Spotify = spotiCredentials,
                    SpotifyUserData = userData,
                    Firebase = token
                };
            }

            return Json(result);
        }

        private RedirectResult AuthorizationCodeFlow()
        {

            var nonce = Guid.NewGuid().ToString("N");

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

    }
}