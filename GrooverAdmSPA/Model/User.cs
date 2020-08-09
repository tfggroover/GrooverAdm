using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    [FirestoreData(ConverterType = typeof(FirestoreEntityConverter<User>))]
    public class User : FirestoreEntity<User>
    {
        public User() {
        }
        //public User(SpotifyUserInfo userInfo, string currentToken, int expiresIn, DateTime dateTime)
        //{
        //    DisplayName = userInfo.Display_name;
        //    Id = userInfo.Id;
        //    CurrentToken = currentToken;
        //    ExpiresIn = expiresIn;
        //    TokenEmissionTime = dateTime;
        //}
        [FirestoreProperty]
        public int Born { get; set; }
        [FirestoreProperty]
        public string DisplayName { get; set; }
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public string CurrentToken { get; set; }
        /// <summary>
        /// Expiration time in seconds
        /// </summary>
        [FirestoreProperty]
        public int ExpiresIn { get; set; }
        public DateTime TokenEmissionTime { get; set; }


    }
}
