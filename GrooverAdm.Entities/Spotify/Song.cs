using System;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using System.Text;

namespace GrooverAdm.Entities.Spotify
{

    public class Song
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<SimplifiedArtist> Artists { get; set; }

    }

    public class PlaylistSong
    {
        public DateTime Added_at { get; set; }
        public bool Is_Local { get; set; }
        public Song Track { get; set; }
    }

    public class GetSongsResponse : PagingWrapper<PlaylistSong>
    {
    }

    public class GetTopSongsResponse : PagingWrapper<Song>
    {

    }
}
