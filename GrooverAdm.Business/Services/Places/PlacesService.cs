﻿using GrooverAdm.Business.Services.Playlist;
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


        public PlacesService(IPlacesDao<DataAccess.Firestore.Model.Place> dao, IPlaceMapper<DataAccess.Firestore.Model.Place> mapper,
            IPlaylistService playlistService)
        {
            _dao = dao;
            _mapper = mapper;
            this.playlistService = playlistService;
        }

        public async Task<Place> CreatePlace(Place place)
        {
            var converted = _mapper.ToDbEntity(place);
            var dbResult = await _dao.CreatePlace(converted);



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
                results.ForEach(async r =>
                {
                    var p = await playlistService.GetMainPlaylistFromPlace(r.Id, true, 1, int.MaxValue);
                    r.MainPlaylist = p;
                });


            return results;
        }


        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, List<Tuple<string, string>> geohashes)
        {
            var dbResult = await _dao.GetPlaces(offset, quantity, geohashes);

            return dbResult.Select(p => _mapper.ToApplicationEntity(p));
        }

        public async Task<Place> UpdatePlace(Place place)
        {
            var converted = _mapper.ToDbEntity(place);
            var dbResult = await _dao.UpdatePlace(converted);

            return _mapper.ToApplicationEntity(dbResult);
        }
    }
}
