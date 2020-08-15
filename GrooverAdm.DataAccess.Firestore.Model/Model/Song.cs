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
            Artists = new List<Artist>();
        }

        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public List<Artist> Artists { get; set; }

    }
}
