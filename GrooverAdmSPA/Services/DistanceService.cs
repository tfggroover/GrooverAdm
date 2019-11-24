using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Services
{
    public class DistanceService
    {

        public static double Distance(GeoPoint point1, GeoPoint point2)
        {
            GeopointService.ValidateGeopoint(point1);
            GeopointService.ValidateGeopoint(point2);

            var R = 6371e3; // metres
            var φ1 = DegreesToRadians(point1.Latitude);
            var φ2 = DegreesToRadians(point2.Latitude);
            var Δφ = DegreesToRadians(point2.Latitude - point1.Longitude);
            var Δλ = DegreesToRadians(point2.Longitude - point1.Longitude);

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var d = R * c;

            return d;
        }


        private static double DegreesToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
