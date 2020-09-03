using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    [FirestoreData(ConverterType = typeof(FirestoreEntityConverter<Place>))]
    public class Place : FirestoreEntity<Place>
    {
        public Place()
        {
            Ratings = new List<Rating>();
            Owners = new List<User>();
            Recognized = new Dictionary<string, int>();
            Timetables = new List<Timetable>();
        }

        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Address { get; set; }
        [FirestoreProperty]
        public string DisplayName { get; set; }
        [FirestoreProperty]
        public GeoPoint Location { get; set; }
        [FirestoreProperty(ConverterType = typeof(FirestoreEntityConverter<Playlist>))]
        public Playlist Playlist { get; set; }
        [FirestoreProperty(ConverterType = typeof(FirestoreEntityListConverter<Rating>))]
        public List<Rating> Ratings { get; set; }
        [FirestoreProperty(ConverterType = typeof(FirestoreEntityListConverter<User>))]
        public List<User> Owners{ get; set; }
        [FirestoreProperty]
        public string Phone { get; set; }
        [FirestoreProperty]
        public string Geohash { get; set; }
        [FirestoreProperty]
        public Dictionary<string, int> Recognized{ get; set; }
        [FirestoreProperty]
        public List<Timetable> Timetables { get; set; }

    }
}
