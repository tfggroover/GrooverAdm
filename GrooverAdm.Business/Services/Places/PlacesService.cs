using GrooverAdm.Business.Services.Playlist;
using GrooverAdm.Business.Services.Song;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.Entities.Application;
using GrooverAdm.Mappers.Firestore;
using GrooverAdm.Mappers.Interface;
using GrooverAdmSPA.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Place = GrooverAdm.Entities.Application.Place;


namespace GrooverAdm.Business.Services.Places
{
    public class PlacesService : IPlacesService
    {
        private readonly IPlacesDao<DataAccess.Firestore.Model.Place> _dao;
        private readonly IPlaceMapper<DataAccess.Firestore.Model.Place> _mapper;
        private readonly IRatingDao<DataAccess.Firestore.Model.Rating> _ratingDao;
        private readonly IRatingMapper<DataAccess.Firestore.Model.Rating> _ratingMapper;
        private readonly IPlaylistService playlistService;
        private readonly ISongService songService;
        private readonly RecommendationService recommendationService;


        public PlacesService(IPlacesDao<DataAccess.Firestore.Model.Place> dao, IPlaceMapper<DataAccess.Firestore.Model.Place> mapper,
            IPlaylistService playlistService, ISongService songService, RecommendationService recommendation)
        {
            _dao = dao;
            _mapper = mapper;
            this.playlistService = playlistService;
            this.songService = songService;
            recommendationService = recommendation;
        }

        public async Task<Place> CreatePlace(Place place)
        {
            var converted = _mapper.ToDbEntity(place);
            var dbResult = await _dao.CreatePlace(converted);

            if (place.MainPlaylist != null)
                await this.playlistService.CreatePlaylist(place.Id, place.MainPlaylist);


            return _mapper.ToApplicationEntity(dbResult);
        }

        public async Task<bool> DeletePlace(string id)
        {
            return await _dao.DeletePlace(id);
        }

        public async Task<Place> GetPlace(string id)
        {
            var dbResult = await _dao.GetPlace(id);

            return _mapper.ToApplicationEntity(dbResult);
        }

        public async Task<IEnumerable<Place>> GetPlaces()
        {
            var dbResult = await _dao.GetPlaces();

            return dbResult.Select(p => _mapper.ToApplicationEntity(p));
        }

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity)
        {
            var dbResult = await _dao.GetPlaces(offset, quantity);

            return dbResult.Select(p => _mapper.ToApplicationEntity(p));
        }

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, Geolocation location, double distance, bool includePlaylist)
        {
            var geohashes = DistanceService.GeohashQueries(location, distance);
            var dbResult = (await _dao.GetPlaces(offset, quantity, geohashes)).Select(p => _mapper.ToApplicationEntity(p));
            
            var results = dbResult.Where(p => DistanceService.Distance(p.Location, location) < distance).ToList();
            if (includePlaylist)
                results.ForEach(r =>
                {
                    var p = playlistService.GetMainPlaylistFromPlace(r.Id, true, 1, int.MaxValue);
                    p.Wait();
                    r.MainPlaylist = p.Result;
                });

            results.ForEach(r =>
            {
                var p = songService.GetRecognizedSongsFromPlace(r.Id);
                p.Wait();
                r.RecognizedMusic = p.Result;
            });

            return results;
        }


        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, List<Tuple<string, string>> geohashes)
        {
            var dbResult = await _dao.GetPlaces(offset, quantity, geohashes);

            return dbResult.Select(p => _mapper.ToApplicationEntity(p));
        }

        public async Task<IEnumerable<ComparedPlace>> GetRecommendedPlaces(int offset, int quantity, Geolocation location, double distance, Entities.Application.Playlist playlist, string spotifyToken)
        {
            var places = (await GetPlaces(1, int.MaxValue, location, distance, true)).Where(p => p.MainPlaylist != null).ToList();

            var compared = (await recommendationService.GetSimilarPlaylistsOrdered(playlist, places, spotifyToken)).Take(quantity).Skip((offset - 1) * quantity).ToList();

            await MassUpdatePlaylists(places.Where(p => p.MainPlaylist.Changed).ToList());

            return compared;
        }

        public async Task<bool> RecognizeSong(string establishmentId, Entities.Application.Song song, string userId)
        {
            var place = await _dao.GetPlace(establishmentId);
            if (place == null)
                return false;
            var res = await songService.RecognizeSong(establishmentId, song, userId);
            if (res != null)
                return true;
            return false;
        }

        public async Task<Place> UpdatePlace(Place place)
        {
            var converted = _mapper.ToDbEntity(place);
            var dbResult = await _dao.UpdatePlace(converted);

            return _mapper.ToApplicationEntity(dbResult);
        }

        public async Task<Entities.Application.Playlist> UpdatePlaylist(Place place, Dictionary<string, int> tags, Dictionary<string, int> genres)
        {
            var playlist = await this.playlistService.GetMainPlaylistFromPlace(place.Id, false, 0, 0);
            playlist.Tags = tags;
            playlist.Genres = genres;
            await this.playlistService.SetPlaylist(place.Id, playlist, false);

            return playlist;
        }

        public async Task<bool> MassUpdatePlaylists(List<Place> places)
        {
            var res = await this.playlistService.SetPlaylists(places);
            if (res.Any())
                return true;
            return false;
        }

    }
}
