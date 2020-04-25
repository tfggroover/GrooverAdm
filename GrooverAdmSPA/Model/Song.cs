using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    [FirestoreData(ConverterType = typeof(FirestoreEntityConverter<Song>))]
    public class Song : FirestoreEntity<Song>
    {
        public Song(){
            Tags = new List<string>();
        }
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public List<string> Tags { get; set; }
        [FirestoreProperty]
        public dynamic Data { get; set; }
    }
}
