using GrooverAdm.Business.Services.Song;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.Entities.Application;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.Playlist
{
    public class PlaylistService : IPlaylistService
    {

        private readonly IPlaylistDao<DataAccess.Firestore.Model.Playlist> _dao;
        private readonly IPlaylistMapper<DataAccess.Firestore.Model.Playlist> _mapper;
        private readonly ISongService _songService;
        private readonly SpotifyService spotify;
       
        public PlaylistService(IPlaylistDao<DataAccess.Firestore.Model.Playlist> dao,
            IPlaylistMapper<DataAccess.Firestore.Model.Playlist> mapper,
            ISongService songService,
            SpotifyService spotify)
        {
            this._dao = dao;
            this._mapper = mapper;
            this._songService = songService;
            this.spotify = spotify;
        }

        public async Task<Entities.Application.Playlist> CreatePlaylist(string place, Entities.Application.Playlist playlist)
        {
            var converted = _mapper.ToDbEntity(playlist, place);
            var dbResult = await _dao.CreatePlaylist(converted);
            var result = _mapper.ToApplicationEntity(dbResult);
            if (playlist.Songs.Any())
                result.Songs = await _songService.AddSongs(playlist.Songs, place, "mainPlaylist");
            

            return result;
        }

        public async Task<bool> DeletePlaylist(string place)
        {
            await _songService.OverrideSongs(new List<Entities.Application.Song>(), place, "mainPlaylist");
            return await _dao.DeletePlaylist(place);
        }

        public async Task<Entities.Application.Playlist> GetMainPlaylistFromPlace(string place, bool includeSongs, int page, int pageSize)
        {
            var dbResult = await _dao.GetPlaylist(place);

            if (dbResult == null)
                return null;
            // TODO ensure playlist is up to date with User token

            // If it is not up to date, update it
            var result = _mapper.ToApplicationEntity(dbResult);
            if (includeSongs)
                result.Songs = await _songService.GetSongsFromPlaylist(place, dbResult.Reference.Id, page, pageSize);
            result.ResetChange();
            return result;
        }

        public async Task<Entities.Application.Playlist> GetPlaylistFromPlace(string place, bool includeSongs)
        {
            throw new NotImplementedException();
            var dbResult = await _dao.GetPlaylist(place);
            // TODO ensure playlist is up to date with User token

            // If it is not up to date, update it
            var result = _mapper.ToApplicationEntity(dbResult);
            if (includeSongs)
                result.Songs = await _songService.GetSongsFromPlaylist(place, dbResult.Reference.Id, 1, int.MaxValue);
            return result;
        }

        public async Task<Entities.Application.Playlist> GetPlaylistFromSpotify(string id, string token)
        {
            id = ParseSpotifyPlaylist(id);
            var playlist = await this.spotify.GetPlaylist(new System.Net.Http.HttpClient(), token, id);
            if (playlist == null)
                return null;
            return new Entities.Application.Playlist
            {
                Id = playlist.Id,
                ImageUrl = playlist.Images.Count > 0 ? playlist.Images[0].Url : "",
                SnapshotVersion = playlist.Snapshot_id,
                Name = playlist.Name,
                Songs = playlist.Tracks.Items.Select(i => new Entities.Application.Song
                {
                    Name = i.Track.Name,
                    Id = i.Track.Id,
                    Artists = i.Track.Artists.Select(a => new Artist { Id = a.Id, Name = a.Name }).ToList()
                }).ToList()
            };
        }

        public static string ParseSpotifyPlaylist(string input)
        {
            var id = input;
            if (Regex.IsMatch(id, "spotify:playlist:\\w+"))
            {
                var split = id.Split(":");
                id = split[split.Length - 1];
            }

            return id;
        }

        public async Task<List<Entities.Application.Playlist>> GetWeeklyPlaylistsFromPlace(string place, bool includeSongs)
        {
            throw new NotImplementedException();
        }

        public async Task<Entities.Application.Playlist> SetPlaylist(string place, Entities.Application.Playlist playlist, bool withSongs)
        {
            var converted = _mapper.ToDbEntity(playlist, place);
            var dbResult =  await _dao.UpdatePlaylist(converted);
            await _songService.OverrideSongs(withSongs ? playlist.Songs : new List<Entities.Application.Song>(), place, "mainPlaylist");

            return _mapper.ToApplicationEntity(dbResult);
        }

        public async Task<List<Entities.Application.Playlist>> SetPlaylists(List<Place> places)
        {

            var playlists = await _dao.UpdatePlaylists(places.Select(p => _mapper.ToDbEntity(p.MainPlaylist, p.Id)).ToList());

            await _songService.OverrideSongsMulti(places);

            return await playlists.ToAsyncEnumerable().SelectAwait(async p => _mapper.ToApplicationEntity(await p)).ToListAsync();
        }
    }
}
