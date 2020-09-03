using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GrooverAdm.Pages
{
    public class AuthPopupModel : PageModel
    {
        public string ApiKey { get; set; }
        public string DbUrl { get; set; }
        public string DisplayName { get; set; }
        public string PhotoUrl { get; set; }
        public string Token { get; set; }
        public string SpotiToken { get; set; }
        public string RefreshToken { get; set; }
        public void OnGet()
        {
        }
    }
}
