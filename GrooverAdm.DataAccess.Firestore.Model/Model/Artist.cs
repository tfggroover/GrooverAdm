using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class Artist
    {
        public Artist() { }
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
    }
}
