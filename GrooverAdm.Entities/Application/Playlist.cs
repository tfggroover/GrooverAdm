using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Playlist : IApplicationEntity
    {
        public Playlist()
        {
            Songs = new List<Song>();
            tags = new Dictionary<string, int>();
            genres = new Dictionary<string, int>();
            Changed = false;
        }

        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        private List<Song> songs;
        public List<Song> Songs
        {
            get { return songs; }
            set
            {
                Changed = true;
                songs = value;
            }
        }
        private Dictionary<string, int> tags;
        [JsonIgnore]
        public Dictionary<string, int> Tags
        {
            get { return tags; }
            set
            {
                Changed = true;
                tags = value;
            }
        }
        private Dictionary<string, int> genres;
        [JsonIgnore]
        public Dictionary<string, int> Genres
        {
            get { return genres; }
            set
            {
                Changed = true;
                genres = value;
            }
        }
        public string SnapshotVersion { get; set; }
        public string Url { get; set; }
        public bool Changed { get; private set; }

        public void ResetChange()
        {
            Changed = false;
        }
    }
}
