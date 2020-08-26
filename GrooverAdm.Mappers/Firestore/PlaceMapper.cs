using Google.Cloud.Firestore;
using GrooverAdm.Entities.Application;
using GrooverAdm.Mappers.Interface;
using System.Linq;

namespace GrooverAdm.Mappers.Firestore
{
    public class PlaceMapper : IPlaceMapper<DataAccess.Firestore.Model.Place>
    {
        private static readonly string COLLECTION_REF = "places";
        private static readonly string USERS_REF = "users";

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
                Phone = entity.Phone,
                Owners = entity.Owners.Select(u => _db.Collection(USERS_REF).Document(u.Id)).ToList(),
                Timetables = entity.Timetables.Select(t => new DataAccess.Firestore.Model.Timetable
                {
                    Day = t.Day,
                    Schedules = t.Schedules.Select(s => new DataAccess.Firestore.Model.Schedule { End = s.End, Start = s.Start }).ToList()
                }).ToList()
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
                Phone = dbEntity.Phone,
                Timetables = dbEntity.Timetables.Select(t => new Timetable
                {
                    Day = t.Day,
                    Schedules = t.Schedules.Select(s => new Schedule { End = s.End, Start = s.Start }).ToList()
                }).ToList(),
                Owners = dbEntity.Owners.Select(u => new User { Id = u.Id }).ToList()
            };
        }
    }
}
