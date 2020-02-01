using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    [FirestoreData(ConverterType = typeof(FirestoreEntityConverter<Playlist>))]
    public class Playlist : FirestoreEntity<Playlist>
    {
        public Playlist()
        {
            Songs = new List<Song>();
        }

        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Hash { get; set; }
        [FirestoreProperty(ConverterType = typeof(FirestoreEntityListConverter<Song>))]
        public List<Song> Songs{ get; set; }
        [FirestoreProperty]
        public dynamic Metrics { get; set; }
    }
}
