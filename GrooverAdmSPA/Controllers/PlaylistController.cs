using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrooverAdmSPA.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrooverAdmSPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        // GET: api/Playlist 
        /// <summary>
        /// Get first N playlists from user
        /// </summary>
        [HttpGet]
        public IEnumerable<Playlist> Get()
        {
            throw new NotImplementedException();
            return new Playlist[] {  };
        }

        // GET: api/Playlist/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(string id)
        {
            return "value";
        }

        // POST: api/Playlist
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Playlist/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
