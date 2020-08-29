using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class User : DataAccess.Model.User
    {
        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }
        [FirestoreProperty]
        public int Born { get; set; }
        [FirestoreProperty]
        public string DisplayName { get; set; }
        [FirestoreProperty]
        public string CurrentToken { get; set; }
        /// <summary>
        /// Expiration time in seconds
        /// </summary>
        [FirestoreProperty]
        public int ExpiresIn { get; set; }
        [FirestoreProperty]
        public DateTime TokenEmissionTime { get; set; }
        [FirestoreProperty]
        public bool Admin { get; set; }
    }
}
