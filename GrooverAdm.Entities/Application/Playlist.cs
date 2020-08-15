using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Playlist : IApplicationEntity
    {
        public Playlist()
        {
            Songs = new List<Song>();
            Tags = new Dictionary<string, int>();
            Genres = new Dictionary<string, int>();
        }

        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public List<Song> Songs{ get; set; }
        public Dictionary<string, int> Tags { get; set; }
        public Dictionary<string, int> Genres { get; set; }
        public string SnapshotVersion { get; set; }
        public string Url { get; set; }
    }
}
