using GrooverAdm.Business.Services.Playlist;
using GrooverAdm.Business.Services.Rating;
using GrooverAdm.Business.Services.Song;
using GrooverAdm.Business.Services.User;
using GrooverAdm.Common;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.Entities.Application;
using GrooverAdm.Mappers.Firestore;
using GrooverAdm.Mappers.Interface;
using GrooverAdmSPA.Business.Services;
using Microsoft.Extensions.Logging;
using NGeoHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Place = GrooverAdm.Entities.Application.Place;


namespace GrooverAdm.Business.Services.Places
{
    public class PlacesService : IPlacesService
    {
        private readonly IPlacesDao<DataAccess.Firestore.Model.Place> _dao;
        private readonly IPlaceMapper<DataAccess.Firestore.Model.Place> _mapper;
        private readonly IRatingService ratingService;
        private readonly IUserService userService;
        private readonly IPlaylistService playlistService;
        private readonly ISongService songService;
        private readonly RecommendationService recommendationService;
        private readonly ILogger log;


        public PlacesService(IPlacesDao<DataAccess.Firestore.Model.Place> dao, IPlaceMapper<DataAccess.Firestore.Model.Place> mapper,
            IPlaylistService playlistService, ISongService songService, RecommendationService recommendation, IUserService users, IRatingService ratingService, ILogger<PlacesService> log)
        {
            _dao = dao;
            _mapper = mapper;
            this.playlistService = playlistService;
            this.songService = songService;
            recommendationService = recommendation;
            userService = users;
            this.ratingService = ratingService;
            this.log = log;
        }

        public async Task<Place> CreatePlace(Place place, string user)
        {
            if (!place.Owners.Any())
                place.Owners.Add(new Entities.Application.ListableUser { Id = user });
            if(place.MainPlaylist != null && place.MainPlaylist.Id != null && string.IsNullOrEmpty(place.MainPlaylist.Name))
            {
                var dbUser = await this.userService.GetUser(user);
                var play = await this.playlistService.GetPlaylistFromSpotify(place.MainPlaylist.Id, dbUser.CurrentToken);
                if (play != null)
                    place.MainPlaylist = play;
            }

            place.Geohash = GeoHash.Encode(place.Location.Latitude, place.Location.Longitude);

            var converted = _mapper.ToDbEntity(place);
            var dbResult = await _dao.CreatePlace(converted);

            if (place.MainPlaylist != null)
                await this.playlistService.CreatePlaylist(dbResult.Reference.Id, place.MainPlaylist);


            return _mapper.ToApplicationEntity(dbResult);
        }



        public async Task<bool> DeletePlace(string id, string user)
        {
            return await _dao.DeletePlace(id, user);
        }

        public async Task<Place> GetPlace(string id)
        {
            var dbResult = await _dao.GetPlace(id);
            var res = _mapper.ToApplicationEntity(dbResult);
            res.MainPlaylist = await playlistService.GetMainPlaylistFromPlace(res.Id, true, 1, int.MaxValue);
            res.Owners = (await userService.GetOwners(res.Owners.Select(o => o.Id))).ToList();
            return res;
        }

        public async Task<IEnumerable<Place>> GetPlaces()
        {
            var dbResult = await _dao.GetPlaces();

            return dbResult.Select(p => _mapper.ToApplicationEntity(p));
        }

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity)
        {
            var dbResult = await _dao.GetPlaces(offset, quantity, false);

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

            compared.ForEach(c =>
            {
                if (double.IsNaN(c.Similitude))
                    c.Similitude = 0;
            });
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

        public async Task<Place> UpdatePlace(Place place, string user)
        {
            var old = await this.GetPlace(place.Id);
            if (!old.Owners.Select(o => o.Id).Contains(user))
                throw new GrooverAuthException("Current User has no permissions to update this place");
            var fullUser = await this.userService.GetUser(user);
            var playlistNeedsUpdate = false;
            var dbFormat = PlaylistService.ParseSpotifyPlaylist(place.MainPlaylist.Id);
            if(old.MainPlaylist?.Id != dbFormat || (!old.MainPlaylist?.Songs?.Any() ?? true))
            {
                old.MainPlaylist = await this.playlistService.GetPlaylistFromSpotify(place.MainPlaylist.Id, fullUser.CurrentToken);
                playlistNeedsUpdate = true;
            }

            old.DisplayName = place.DisplayName;
            old.Location = place.Location;
            old.Address = place.Address;
            old.PendingReview = !old.Approved;
            old.Phone = place.Phone;
            old.Timetables = place.Timetables;
            old.Geohash = GeoHash.Encode(old.Location.Latitude, old.Location.Longitude);

            var converted = _mapper.ToDbEntity(old);
            var dbResult = await _dao.UpdatePlace(converted);
            if (playlistNeedsUpdate)
                await playlistService.SetPlaylist(old.Id, old.MainPlaylist, true);

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

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, string user, bool onlyUser, bool pendingReview)
        {
            IEnumerable<DataAccess.Firestore.Model.Place> res;
            if (onlyUser)
                res = await _dao.GetPlaces(offset, quantity, user, pendingReview);
            else
                res = await _dao.GetPlaces(offset, quantity, pendingReview);
            return res.Select(p => _mapper.ToApplicationEntity(p));
        }

        public async Task<Place> RatePlace(string placeId, double value, string user)
        {
            var res = await this.ratingService.RatePlace(placeId, value, user);
            var place = await this._dao.GetPlace(placeId);
            if (res.New)
            {
                place.RatingTotal = ((place.RatingTotal * place.RatingCount) + value) / (place.RatingCount + 1);
                place.RatingCount++;
            } 
            else
            {
                place.RatingTotal = ((place.RatingTotal * place.RatingCount) - res.OldValue.Value + value) / place.RatingCount;
            }
            var result = await this._dao.UpdatePlace(place);

            return _mapper.ToApplicationEntity(result);
        }

        public async Task<Place> ReviewPlace(string placeId, PlaceReview review, string user)
        {
            var dbUser = await userService.GetUser(user);
            if(!dbUser.Admin)
                throw new GrooverAuthException($"The current user is not an admin, user id: {user}");
            var res = await _dao.ReviewPlace(placeId, review.Approved, review.ReviewComment);

            return _mapper.ToApplicationEntity(res);
        }
    }
}
