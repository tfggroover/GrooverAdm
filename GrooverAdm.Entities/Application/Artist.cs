using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Entities.Application
{
    public class Artist
    {
        public Artist() { }
        public Artist(Spotify.SimplifiedArtist artist)
        {
            Id = artist.Id;
            Name = artist.Name;
        }
        public string Id { get; set; }
        public string Name { get; set; }

    }
}
