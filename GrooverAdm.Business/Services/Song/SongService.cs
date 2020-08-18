using GrooverAdm.DataAccess.Dao;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.Song
{
    public class SongService: ISongService
    {
        private readonly ISongDao<DataAccess.Firestore.Model.Song> _dao;
        private readonly ISongMapper<DataAccess.Firestore.Model.Song> _mapper;
        private readonly SpotifyService spotify;

        public SongService(ISongDao<DataAccess.Firestore.Model.Song> dao, ISongMapper<DataAccess.Firestore.Model.Song> mapper, SpotifyService spotify)
        {
            _dao = dao;
            _mapper = mapper;
            this.spotify = spotify;
        }

        public async Task<Entities.Application.Song> AddSong(Entities.Application.Song song)
        {
            var converted = _mapper.ToDbEntity(song);
            var dbResult = await _dao.AddSong(converted);

            return _mapper.ToApplicationEntity(dbResult);
        }

        public async Task<List<Entities.Application.Song>> AddSongs(List<Entities.Application.Song> songs, string place, string playlist)
        {
            var converted = songs.Select(s => _mapper.ToDbEntity(s, place, playlist)).ToList();
            var dbResult = await _dao.AddSongs(converted);
            var result = new List<Entities.Application.Song>();
            
            await dbResult.ToAsyncEnumerable().ForEachAsync(async r => result.Add(_mapper.ToApplicationEntity(await r)));
            return result;
        }

        public async Task<List<Entities.Application.Song>> GetSongsFromPlaylist(string place, string playlist, int page, int pageSize)
        {
            var dbResult = await _dao.GetSongs(place, playlist, page, pageSize);

            return dbResult.Select(r => _mapper.ToApplicationEntity(r)).ToList();
        }

        public async Task<List<Entities.Application.Song>> OverrideSongs(List<Entities.Application.Song> songs, string place, string playlist)
        {
            var converted = songs.Select(s => _mapper.ToDbEntity(s, place, playlist)).ToList();
            var dbResult = await _dao.OverrideSongs(converted, place, playlist);
            var result = new List<Entities.Application.Song>();

            await dbResult.ToAsyncEnumerable().ForEachAsync(async r => result.Add(_mapper.ToApplicationEntity(await r)));
            return result;
        }

        public async Task<Entities.Application.Song> RecognizeSong(string establishmentId, Entities.Application.Song song)
        {
            var converted = _mapper.ToDbEntity(song, establishmentId);
            var dbResult = await _dao.AddSong(converted);
            var result = _mapper.ToApplicationEntity(dbResult);

            return result;
        }
    }
}
