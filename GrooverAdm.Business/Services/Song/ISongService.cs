﻿using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.Song
{
    public interface ISongService
    {
        Task<List<Entities.Application.Song>> GetSongsFromPlaylist(string place, string playlist, int page, int pageSize);
        Task<List<Entities.Application.Song>> AddSongs(List<Entities.Application.Song> songs, string place, string playlist);
        Task<List<Entities.Application.Song>> OverrideSongs(List<Entities.Application.Song> songs, string place, string playlist);
        Task<Entities.Application.Song> AddSong(Entities.Application.Song song);
        Task<Entities.Application.Song> RecognizeSong(string establishmentId, Entities.Application.Song songId, string userId);
        Task<List<Entities.Application.Song>> OverrideSongsMulti(List<Place> places);
        Task<List<RecognizedSong>> GetRecognizedSongsFromPlace(string id);
    }
}
