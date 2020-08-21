using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Entities.Application
{
    public class Geolocation
    {
        public Geolocation() { }
        public Geolocation(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
