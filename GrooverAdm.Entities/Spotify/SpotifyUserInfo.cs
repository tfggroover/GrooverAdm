using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Spotify
{
    public class SpotifyUserInfo
    {
        public string Country { get; set; }
        public string Display_name { get; set; }
        public string Email { get; set; }
        public ExplicitContentFilters Explicit_content { get; set; }
        public Dictionary<string,string> External_urls { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }
        public List<Image> Images { get; set; }
        public string Product { get; set; } //Probablemente sea mejor un enum
        public string Type { get; set; }
        public string Uri { get; set; } //Solo útil en un móvil

    }

    public class ExplicitContentFilters
    {
        public bool Filter_enabled { get; set; }
        public bool Filter_locked { get; set; }
    }

    public class Image
    {
        public int? Height { get; set; }
        public int? Width { get; set; }
        public string Url { get; set; }
    }
}
