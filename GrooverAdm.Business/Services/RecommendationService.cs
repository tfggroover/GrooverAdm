using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Business.Services
{
    public class RecommendationService
    {
        private readonly SpotifyService _spotify;
        
        public RecommendationService(SpotifyService spotify)
        {
            _spotify = spotify;
        }


        public List<Entities.Application.Playlist> GetSimilarPlaylistsOrdered()
        {
            return null;
        }

    }
}
