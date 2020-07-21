using Google.Cloud.Firestore;
using GrooverAdm.Entities.Application;
using NGeoHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Business.Services
{
    public class DistanceService
    {
        private const int METERS_PER_DEGREE_LATITUDE = 110574;
        private const string BASE32 = "0123456789bcdefghjkmnpqrstuvwxyz";
        private const double EARTH_EQ_RADIUS = 6378137.0;
        private const double E2 = 0.00669447819799;
        public const int GEOHASH_PRECISION = 10;
        private const double EPSILON = 1e-12;
        private const int BITS_PER_CHAR = 5;
        private const int MAXIMUM_BITS_PRECISION = 22 * BITS_PER_CHAR;
        private const int EARTH_MERI_CIRCUMFERENCE = 40007860;


        public static double Distance(Geolocation point1, Geolocation point2)
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

        /// <summary>
        /// Calculates eight points on the bounding box and the center of a given circle. At least one
        /// geohash of these nine coordinates, truncated to a precision of at most radius, are guaranteed
        /// to be prefixes of any geohash that lies within the circle.
        /// </summary>
        /// <param name="center">The center</param>
        /// <param name="radius">The radius specified in metres</param>
        /// <returns>The eight bounding box points (And the center)</returns>
        public static List<Coordinates> BoundingBoxCoordinates(Geolocation center, double radius)
        {
            var latitude = radius / METERS_PER_DEGREE_LATITUDE;
            var latitudeNorth = Math.Min(90, center.Latitude + latitude);
            var latitudeSouth = Math.Max(-90, center.Latitude - latitude);
            var longDegsNorth = MetersToLongitudeDegrees(radius, latitudeNorth);
            var longDegsSouth = MetersToLongitudeDegrees(radius, latitudeSouth);
            var longDegs = Math.Max(longDegsNorth, longDegsSouth);
            return new List<Coordinates>()
            {
                new Coordinates(){ Lat = center.Latitude, Lon = center.Longitude},
                new Coordinates(){ Lat = center.Latitude, Lon = center.Longitude - longDegs},
                new Coordinates(){ Lat = center.Latitude, Lon = center.Longitude + longDegs},
                new Coordinates(){ Lat = latitudeNorth, Lon = center.Longitude},
                new Coordinates(){ Lat = latitudeNorth, Lon = center.Longitude - longDegs},
                new Coordinates(){ Lat = latitudeNorth, Lon = center.Longitude + longDegs},
                new Coordinates(){ Lat = latitudeSouth, Lon = center.Longitude},
                new Coordinates(){ Lat = latitudeSouth, Lon = center.Longitude - longDegs},
                new Coordinates(){ Lat = latitudeSouth, Lon = center.Longitude + longDegs}
            };
        }


        /// <summary>
        /// Calculates the number of degrees a given distance is at a given latitude.
        /// </summary>
        /// <param name="distance">The distance to convert.</param>
        /// <param name="latitude">The latitude at which to calculate.</param>
        /// <returns>The number of degrees the distance corresponds to.</returns>
        public static double MetersToLongitudeDegrees(double distance, double latitude)
        {
            var radians = DegreesToRadians(latitude);
            var num = Math.Cos(radians) * EARTH_EQ_RADIUS * Math.PI / 180;
            var denom = 1 / Math.Sqrt(1 - E2 * Math.Sin(radians) * Math.Sin(radians));
            var deltaDeg = num * denom;
            if (deltaDeg < EPSILON)
            {
                return distance > 0 ? 360 : 0;
            }
            else
            {
                return Math.Min(360, distance / deltaDeg);
            }
        }

        public static List<Tuple<string, string>> GeohashQueries(Geolocation center, double radius)
        {
            GeopointService.ValidateGeopoint(center);
            var queryBits = Math.Max(1, BoundingBoxBits(center, radius));
            var geohashPrecision = queryBits / BITS_PER_CHAR;
            var coordinates = BoundingBoxCoordinates(center, radius);
            var queries = coordinates.Select((coordinate) =>
            {
                return GeohashQuery(GeoHash.Encode(coordinate.Lat, coordinate.Lon, geohashPrecision), queryBits);
            });
            // remove duplicates
            return queries.ToHashSet().ToList();
        }

        private static Tuple<string, string> GeohashQuery(string geohash, int bits)
        {
            ValidateGeohash(geohash);
            var precision = bits / BITS_PER_CHAR;
            if (geohash.Length < precision)
            {
                return new Tuple<string, string>(geohash, geohash + '~');
            }
            geohash = geohash.Substring(0, precision);
            var basic = geohash.Substring(0, geohash.Length - 1);
            var lastValue = BASE32.IndexOf(geohash[geohash.Length - 1]);
            var significantBits = bits - (basic.Length * BITS_PER_CHAR);
            var unusedBits = (BITS_PER_CHAR - significantBits);
            // delete unused bits
            var startValue = (lastValue >> unusedBits) << unusedBits;
            var endValue = startValue + (1 << unusedBits);
            if (endValue > 31)
            {
                return new Tuple<string, string>(basic + BASE32[startValue], basic + '~');
            }
            else
            {
                return new Tuple<string, string>(basic + BASE32[startValue], basic + BASE32[endValue]);
            }
        }

        /// <summary>
        /// Validates the inputted geohash and throws an error if it is invalid.
        /// </summary>
        /// <param name="geohash">The geohash to be Validated</param>
        private static void ValidateGeohash(string geohash)
        {
            if(string.IsNullOrWhiteSpace(geohash))
                throw new ArgumentException("Geohash cannot be an empty string");
            else
                foreach (char a in geohash)
                {
                    if (BASE32.IndexOf(a) == -1)
                        throw new ArgumentException($"Geohash cannot contain {a}");
                }
        }

        private static int BoundingBoxBits(Geolocation coordinate, double radius)
        {
            var latDeltaDegrees = radius/ METERS_PER_DEGREE_LATITUDE;
            var latitudeNorth = Math.Min(90, coordinate.Latitude + latDeltaDegrees);
            var latitudeSouth = Math.Max(-90, coordinate.Latitude - latDeltaDegrees);
            var bitsLat = Math.Floor(LatitudeBitsForResolution(radius)) * 2;
            var bitsLongNorth = Math.Floor(LongitudeBitsForResolution(radius, latitudeNorth)) * 2 - 1;
            var bitsLongSouth = Math.Floor(LongitudeBitsForResolution(radius, latitudeSouth)) * 2 - 1;
            return (int) Math.Min(Math.Min(bitsLat, bitsLongNorth), Math.Min(bitsLongSouth, MAXIMUM_BITS_PRECISION));
        }

        /// <summary>
        /// Calculates the bits necessary to reach a given resolution, in meters, for the longitude at a
        /// given radius
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="latitudeNorth"></param>
        /// <returns></returns>
        private static double LongitudeBitsForResolution(double radius, double latitudeNorth)
        {
            var degs = MetersToLongitudeDegrees(radius, latitudeNorth);
            return Math.Abs(degs) > 0.000001 ? Math.Max(1, Log2(360/degs)) : 1 ;
        }

        private static double LatitudeBitsForResolution(double radius)
        {
            return Math.Min(Log2(EARTH_MERI_CIRCUMFERENCE / 2 / radius), MAXIMUM_BITS_PRECISION);
        }

        private static double Log2(double x)
        {
            return Math.Log(x) / Math.Log(2);
        }
    }
}