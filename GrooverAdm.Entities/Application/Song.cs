using GrooverAdm.Entities.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Song : IApplicationEntity 
    {
        public Song()
        {
            Artists = new List<Artist>();
        }
        public Song(PlaylistSong song)
        {
            Id = song.Track.Id;
            Name = song.Track.Name;
            Artists = song.Track.Artists.Select(a => new Artist(a)).ToList();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Artist> Artists { get; set; }
    }

    public class RecognizedSong: Song
    {
        [JsonIgnore]
        public HashSet<string> Users { get; set; }
        public int Count { get; set; }
    }
}
