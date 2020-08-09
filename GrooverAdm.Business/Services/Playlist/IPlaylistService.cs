using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.Playlist
{
    public interface IPlaylistService
    {
        Task<Entities.Application.Playlist> GetMainPlaylistFromPlace(string place, bool includeSongs, int page, int pageSize);
        Task<List<Entities.Application.Playlist>> GetWeeklyPlaylistsFromPlace(string place, bool includeSongs);
        Task<Entities.Application.Playlist> SetPlaylist(string place, Entities.Application.Playlist playlist);
        Task<Entities.Application.Playlist> CreatePlaylist(string place, Entities.Application.Playlist playlist);
        Task<bool> DeletePlaylist(string place);

    }
}
