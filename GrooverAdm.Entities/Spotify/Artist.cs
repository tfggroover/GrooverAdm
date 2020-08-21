using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Text;

namespace GrooverAdm.Entities.Spotify
{
    public class SimplifiedArtist
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class Artist
    {
        public HashSet<string> Genres { get; set; }
        public string Id { get; set; }
    }

    public class ArtistResponse
    {
        public List<Artist> Artists { get; set; }
    }
}
