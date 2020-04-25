using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Services
{
    public class GeopointService
    {


        public static void ValidateGeopoint(GeoPoint point){
            string error ="";


            var latitude = point.Latitude;
            var longitude = point.Longitude;

            if (latitude < -90 || latitude > 90)
            {
                error = "latitude must be within the range [-90, 90]";
            }
            else if (longitude < -180 || longitude > 180)
            {
                error = "longitude must be within the range [-180, 180]";
            }
            
            if (!string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentException("Invalid GeoFire location \'" + point.ToString() + "\': " + error);
            }
        }
    }
}
