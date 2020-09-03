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
using GrooverAdm.Common;

namespace GrooverAdm.DataAccess.Firestore.PlacesDao
{
    public class PlacesFirestoreDao : IPlacesDao<Place>
    {
        private readonly FirestoreDb _db;

        private static readonly string COLLECTION_REF = "places";
        private static readonly string USERS_REF = "users";

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

        public  async Task<bool> DeletePlace(string id, string user)
        {
            var userRef = _db.Collection(USERS_REF).Document(user);
            var reference = _db.Collection(COLLECTION_REF).Document(id);
            var snap = await reference.GetSnapshotAsync();
            if (snap.Exists && snap.ConvertTo<Place>().Owners.Contains(userRef))
            {
                var res = await reference.DeleteAsync();
                if (res.UpdateTime != null)
                    return true;
                return false;
            }
            else if (!snap.ConvertTo<Place>().Owners.Contains(userRef))
                throw new GrooverAuthException("The current user is not allowed to delete this place");
            return false;
        }

        public async Task<Place> GetPlace(string id)
        {
            var snap = await _db.Collection(COLLECTION_REF).Document(id).GetSnapshotAsync();
            if(snap.Exists)
                return snap.ConvertTo<Place>();

            return null;
        }

        public async Task<IEnumerable<Place>> GetPlaces()
        {
            var res = await _db.Collection(COLLECTION_REF).ListDocumentsAsync().SelectAwait(async r => (await r.GetSnapshotAsync()).ConvertTo<Place>()).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, bool onlyPending)
        {
            if (onlyPending)
            {
                var result = (await _db.Collection(COLLECTION_REF).WhereEqualTo(nameof(Place.PendingReview), onlyPending).Limit(quantity).Offset((offset - 1) * quantity).GetSnapshotAsync()).Select(r => r.ConvertTo<Place>()).ToList();
                return result;
            }
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
                    .WhereLessThanOrEqualTo("geohash", hash.Item2).Limit(quantity).Offset((offset -1) * quantity)) // Create the queries
                .Select(async a => {
                    var snapshot = await a.GetSnapshotAsync();
                    return snapshot;
                }) //Get the snapshot
                .SelectMany(r => r.Result.Documents.Select(doc => doc.ConvertTo<Place>())) //Resolve the establishments
                .Take(quantity)
                .Skip((offset - 1) * quantity)
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

        public async Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, string user, bool onlyPending)
        {
            var userRef = _db.Collection(USERS_REF).Document(user);
            var baseQuery = _db.Collection(COLLECTION_REF).WhereArrayContains("Owners", userRef);
            if (onlyPending)
                baseQuery = baseQuery.WhereEqualTo(nameof(Place.PendingReview), onlyPending);
            var result = await baseQuery.Limit(quantity).Offset((offset - 1) * quantity)
                .GetSnapshotAsync();

            return result.Select(d => d.ConvertTo<Place>());
        }

        public async Task<Place> ReviewPlace(string placeId, bool approved, string reviewComment)
        {
            var reference = _db.Collection(COLLECTION_REF).Document(placeId);
            var snap = await reference.GetSnapshotAsync();
            if (!snap.Exists)
                return null;
            var place = snap.ConvertTo<Place>();
            place.Approved = approved;
            place.ReviewComment = reviewComment;
            place.PendingReview = false;
            await reference.SetAsync(place);
            return place;
        }
    }
}
