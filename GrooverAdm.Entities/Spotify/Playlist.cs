using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Entities.Spotify
{
    public class Playlist
    {
        public Playlist()
        {
            Images = new List<Image>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Snapshot_id { get; set; }
        public List<Image> Images { get; set; }
        public GetSongsResponse Tracks { get; set; }
    }

    public class GetPlaylistsResponse: PagingWrapper<Playlist>
    {
    }
}
