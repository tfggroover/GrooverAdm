using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrooverAdmSPA.Model;
using Microsoft.AspNetCore.Mvc;

namespace GrooverAdmSPA.Controllers
{
    [Route("api/[controller]")]
    public class GenreController : Controller
    {
        [HttpGet]
        public IActionResult Index(List<Song> songs)
        {

            throw new NotImplementedException("Jokes on you");
            return Ok();
        }
    }
}