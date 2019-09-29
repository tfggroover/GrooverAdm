using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace GrooverAdmSPA.Controllers
{
    [Route("[Controller]")]
    public class HomeController : Controller
    {


        private readonly IConfiguration Configuration;

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Redirect")]
        public IActionResult Redirect()
        {
            
            var cookieNonce = Request.Cookies["State"];
            string nonce;
            if (string.IsNullOrEmpty(cookieNonce)) {
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

        [HttpGet("callback")]
        public IActionResult AuthCallback(string code)
        {
            if (string.IsNullOrEmpty(Request.Cookies["State"]))
            {
                return BadRequest("State cookie not set or expired. Maybe you took too long to authorize. Please try again.");
            }
            else if(Request.Cookies["State"] != Request.Query["State"])
            {
                return BadRequest("State verification failed.");
            }

            var responseString = "";

            if (code.Length > 0)
            {
                using (HttpClient client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Configuration["ClientID"] + ":" + Configuration["ClientSecret"])));

                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("redirect_uri", Configuration["RedirectURI"]),
                        new KeyValuePair<string, string>("grant_type", "authorization_code")
                    });

                    var response = client.PostAsync($"{Configuration["AccessEndpoint"]}", formContent).Result;

                    var responseContent = response.Content;
                    responseString = responseContent.ReadAsStringAsync().Result;
                }
            }

            return Json(responseString);
        }

        public IActionResult AuthResponse(string access_token, string token_type, string expires_in, string state)
        {
            throw new NotImplementedException();
        }
    }
}