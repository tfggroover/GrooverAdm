using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Place : IApplicationEntity
    {
        public Place()
        {
            Ratings = new List<Rating>();
            Owners = new List<User>();
            Recognized = new Dictionary<string, int>();
            Timetables = new List<Timetable>();
        }

        public string Id { get; set; }
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public Geolocation Location { get; set; }
        public Playlist Playlist { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<User> Owners{ get; set; }
        public string Phone { get; set; }
        public string Geohash { get; set; }
        public Dictionary<string, int> Recognized{ get; set; }
        public List<Timetable> Timetables { get; set; }

    }
}
