using Google.Cloud.Firestore;
using GrooverAdm.Entities.Application;
using GrooverAdm.Mappers.Interface;

namespace GrooverAdm.Mappers.Firestore
{
    public class PlaceMapper : IPlaceMapper<DataAccess.Firestore.Model.Place>
    {
        private static readonly string COLLECTION_REF = "places";

        private readonly FirestoreDb _db;
        public PlaceMapper(FirestoreDb db)
        {
            _db = db;
        }

        public DataAccess.Firestore.Model.Place ToDbEntity(Place entity)
        {
            DocumentReference reference = null;
            if (!string.IsNullOrEmpty(entity.Id))
                reference = _db.Collection(COLLECTION_REF).Document(entity.Id);
            return new DataAccess.Firestore.Model.Place
            {
                Reference = reference,
                Address = entity.Address,
                DisplayName = entity.DisplayName,
                Geohash = entity.Geohash,
                Location = new GeoPoint(entity.Location.Latitude, entity.Location.Longitude),
                Phone = entity.Phone
            };
        }

        public Place ToApplicationEntity(DataAccess.Firestore.Model.Place dbEntity)
        {
            return new Place
            {
                Id = dbEntity.Reference.Id,
                Address = dbEntity.Address,
                DisplayName = dbEntity.DisplayName,
                Location = new Geolocation
                {
                    Latitude = dbEntity.Location.Latitude,
                    Longitude = dbEntity.Location.Longitude
                },
                Geohash = dbEntity.Geohash,
                Phone = dbEntity.Phone
            };
        }
    }
}
