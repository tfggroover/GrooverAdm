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

        public PlacesService(IPlacesDao<DataAccess.Firestore.Model.Place> dao, IPlaceMapper<DataAccess.Firestore.Model.Place> mapper)
        {
            _dao = dao;
            _mapper = mapper;
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

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, Geolocation location, double distance)
        {
            var geohashes = DistanceService.GeohashQueries(location, distance);
            var dbResult = (await _dao.GetPlaces(offset, quantity, geohashes)).Select(p => _mapper.ToApplicationEntity(p));
            
            return dbResult.Where(p => DistanceService.Distance(p.Location, location) < distance).ToList();
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
