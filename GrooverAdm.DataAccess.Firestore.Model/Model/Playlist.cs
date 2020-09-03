using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class Playlist : DataAccess.Model.Playlist
    {

        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string ImageUrl { get; set; }
        [FirestoreProperty]
        public string SnapshotId { get; set; }
        [FirestoreProperty]
        public string Url { get; set; }
        [FirestoreProperty]
        public Dictionary<string, int> LastFmOcurrenceDictionary { get; set; }
        [FirestoreProperty]
        public Dictionary<string, int> SpotifyOccurrenceDictionary { get; set; }
    }
}
