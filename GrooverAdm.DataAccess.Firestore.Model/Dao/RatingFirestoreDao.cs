using Google.Cloud.Firestore;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Firestore.Dao
{
    public class RatingFirestoreDao : IRatingDao<Rating>
    {
        private readonly FirestoreDb _db;
        private readonly string PLACES_REF = "places";
        private readonly string RATINGS_REF = "ratings";
        public RatingFirestoreDao(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<Rating> CreateOrUpdateRating(Rating rating, string placeId)
        {
            var snap = await rating.Reference.GetSnapshotAsync();
            if (snap.Exists)
            {
                var old = snap.ConvertTo<Rating>();
                await rating.Reference.UpdateAsync(new Dictionary<string, object> { { "Value", rating.Value } });
                rating.OldValue = old.Value;
            }
            else
            {
                await rating.Reference.CreateAsync(rating);
                rating.New = true;
            }

            return rating;
        }

        public async Task<bool> DeleteRating(string placeId, string ratingId)
        {
            var refer = _db.Collection(PLACES_REF).Document(placeId).Collection(RATINGS_REF).Document(ratingId);
            if((await refer.GetSnapshotAsync()).Exists)
            {
                await refer.DeleteAsync();
                return true;
            }

            return false;
        }
    }
}
