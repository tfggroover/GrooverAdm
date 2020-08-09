using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class Song: DataAccess.Model.Song
    {
        public Song()
        {
            Tags = new List<string>();
        }

        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }
        [FirestoreProperty]
        public List<string> Tags { get; set; }
    }
}
