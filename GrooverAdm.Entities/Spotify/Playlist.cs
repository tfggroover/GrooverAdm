using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Entities.Spotify
{
    public class Playlist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SnapshotVersion { get; set; }

    }

    public class GetPlaylistsResponse: PagingWrapper<Playlist>
    {
    }
}
