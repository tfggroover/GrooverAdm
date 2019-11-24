using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public class User : FirestoreEntity
    {
        public User() { }
        public User(SpotifyUserInfo userInfo)
        {
            DisplayName = userInfo.Display_name;
            Id = userInfo.Id;
        }
        public int Born { get; set; }
        public string DisplayName { get; set; }
        public string Id { get; set; }
        public List<Establishment> Establishments { get; set; }

    }
}
