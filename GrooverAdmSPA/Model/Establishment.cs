using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public class Establishment : FirestoreEntity
    {
        public string DisplayName { get; set; }
        public GeoPoint Location { get; set; }
        public string Playlist { get; set; }
        public List<Rating> Ratings { get; set; }
        public User Owner{ get; set; }
        public string Phone { get; set; }
        public string Geohash { get; set; }
        public List<Recognition> Recognized { get; set; }

    }
}
