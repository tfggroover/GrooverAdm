using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class Rating: DataAccess.Model.Rating
    {
        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }
        [FirestoreProperty]
        public double Value { get; set; }
        public bool New { get; set; }
        public double OldValue { get; set; }
    }
}
