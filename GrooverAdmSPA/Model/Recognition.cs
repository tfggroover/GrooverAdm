using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public class Recognition : FirestoreEntity
    {
        public string Id { get; set; }
        public GeoPoint Location { get; set; }
        public string Geohash { get; set; }
        public Dictionary<string,int> Songs { get; set; }
    }
}
