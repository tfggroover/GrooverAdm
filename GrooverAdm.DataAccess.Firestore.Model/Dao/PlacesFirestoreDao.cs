using Google.Cloud.Firestore;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Place = GrooverAdm.DataAccess.Firestore.Model.Place;

namespace GrooverAdm.DataAccess.Firestore.PlacesDao
{
    public class PlacesFirestoreDao : IPlacesDao<Place>
    {
        private readonly FirestoreDb _db;

        private static readonly string COLLECTION_REF = "places";

        public  PlacesFirestoreDao(FirestoreDb db)
        {
            _db = db;
        }

        public  async Task<Place> CreatePlace(Place place)
        {
            var reference = await _db.Collection(COLLECTION_REF).AddAsync(place);

            var snap = await reference.GetSnapshotAsync();
            return snap.ConvertTo<Place>();
        }

        public  async Task<bool> DeletePlace(string id)
        {
            var res = await _db.Collection(COLLECTION_REF).Document(id).DeleteAsync();
            if (res.UpdateTime != null)
                return true;

            return false;
        }

        public async Task<Place> GetPlace(string id)
        {
            var snap = await _db.Collection(COLLECTION_REF).Document(id).GetSnapshotAsync();
            var res = snap.ConvertTo<Place>();

            return res;
        }

        public async Task<IEnumerable<Place>> GetPlaces()
        {
            var res = await _db.Collection(COLLECTION_REF).ListDocumentsAsync().SelectAwait(async r => (await r.GetSnapshotAsync()).ConvertTo<Place>()).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity)
        {
            var res = await _db.Collection(COLLECTION_REF).ListDocumentsAsync().Take(quantity).Skip(offset).SelectAwait(async r => (await r.GetSnapshotAsync()).ConvertTo<Place>()).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, Geolocation location, double distance)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, List<Tuple<string,string>> geohashes)
        {
            var res = geohashes
                .Select(hash =>
                    _db.Collection(COLLECTION_REF).WhereGreaterThanOrEqualTo("geohash", hash.Item1)
                    .WhereLessThanOrEqualTo("geohash", hash.Item2)) // Create the queries
                .Select(async a => {
                    var snapshot = await a.GetSnapshotAsync();
                    return snapshot;
                }) //Get the snapshot
                .SelectMany(r => r.Result.Documents.Select(doc => doc.ConvertTo<Place>())) //Resolve the establishments
                .ToHashSet(); //Erase duplicates

            return res;
        }

        public async Task<Place> UpdatePlace(Place place)
        {
            var res = await place.Reference.SetAsync(place);

            if (res.UpdateTime != null)
                return place;
            return null;
        }
    }
}
