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
        public User(SpotifyUserInfo userInfo)
        {
            DisplayName = userInfo.Display_name;
            Id = userInfo.Id;
        }
        [FirestoreProperty]
        public int Born { get; set; }
        [FirestoreProperty]
        public string DisplayName { get; set; }
        [FirestoreDocumentId]
        public string Id { get; set; }


    }
}
