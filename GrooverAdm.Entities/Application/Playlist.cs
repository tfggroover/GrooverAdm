using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Playlist
    {
        public Playlist()
        {
            Songs = new List<Song>();
        }

        public string Id { get; set; }
        public string Hash { get; set; }
        public List<Song> Songs{ get; set; }
        public dynamic Metrics { get; set; }
    }
}
