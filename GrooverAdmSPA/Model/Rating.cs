using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    [FirestoreData(ConverterType = typeof(FirestoreEntityConverter<Rating>))]
    public class Rating : FirestoreEntity<Rating>
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public double Value { get; set; }
        [FirestoreProperty(ConverterType = typeof(FirestoreEntityConverter<User>))]
        public User User { get; set; }
    }
}
