using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GrooverAdmSPA.Model;
using GrooverAdmSPA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooverAdmSPA.Controllers
{
    [Route("api/[controller]")]
    [Authorize()]
    public class PlaceController : Controller
    {
        private readonly FirestoreDb _db;
        public PlaceController(FirestoreDb db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<IEnumerable<Place>> GetEstablishments(double lat, double lon, double distance)
        {
            var center = new GeoPoint(lat, lon);
            var geohashes = DistanceService.GeohashQueries(center, distance);
            var places = _db.Collection("places");

#if DEBUG
            await places.ListDocumentsAsync().ForEachAsync(async d =>
            {
                var doc = (await d.GetSnapshotAsync());
                var est = doc.ConvertTo<Place>();
                if (!est.Timetables.Any() && doc.ToDictionary().ContainsKey("open"))
                    foreach (var t in doc.GetValue<Dictionary<string, string>>("open"))
                    {
                        DayOfWeek day;
                        switch (t.Key.ToLower())
                        {
                            case "monday":
                                day = DayOfWeek.Monday;
                                break;
                            case "tuesday":
                                day = DayOfWeek.Tuesday;
                                break;
                            case "wednesday":
                                day = DayOfWeek.Wednesday;
                                break;
                            case "thursday":
                                day = DayOfWeek.Thursday;
                                break;
                            case "friday":
                                day = DayOfWeek.Friday;
                                break;
                            case "saturday":
                                day = DayOfWeek.Saturday;
                                break;
                            case "sunday":
                                day = DayOfWeek.Sunday;
                                break;
                            default:
                                throw new ArgumentException("In what type of week are you writing mate?");
                        }
                        var times = t.Value.Split('-');
                        if (times.Length != 2)
                        {
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            est.Timetables.Add(new Timetable()
                            {
                                Day = day,
                                Id = Guid.NewGuid().ToString()
                            });
                        }
                        else
                        {
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            est.Timetables.Add(new Timetable()
                            {
                                Day = day,
                                Schedules = new List<Schedule>
                                {
                                    new Schedule
                                    {
                                        Start = DateTime.ParseExact(times[0].Trim(), "HH:mm", provider),
                                        End = DateTime.ParseExact(times[1].Trim(), "HH:mm", provider)
                                    }
                                },

                                Id = Guid.NewGuid().ToString()
                            });
                        }
                    }
                if (!est.Recognized.Any() && doc.ToDictionary().ContainsKey("recognized"))
                {
                    var recognizedSongs = await _db.Collection("music_recognition").Document(doc.GetValue<string>("recognized")).GetSnapshotAsync();
                    if (recognizedSongs.ToDictionary().ContainsKey("songs"))
                    {
                        est.Recognized = recognizedSongs.GetValue<Dictionary<string, int>>("songs");
                    }
                }
                if (est.Playlist == null && doc.ToDictionary().ContainsKey("playlist"))
                    est.Playlist = new Playlist
                    {
                        Id = doc.GetValue<string>("playlist")
                    };
                if (string.IsNullOrEmpty(est.Geohash))
                    est.Geohash = NGeoHash.GeoHash.Encode(est.Location.Latitude, est.Location.Longitude, DistanceService.GEOHASH_PRECISION);
                await d.SetAsync(est);
            });
#endif
            //TODO check
            var uniquePlaces = geohashes
                .Select(hash =>
                    places.WhereGreaterThanOrEqualTo("geohash", hash.Item1)
                    .WhereLessThanOrEqualTo("geohash", hash.Item2)) // Create the queries
                .Select(async a => {
                    var snapshot = await a.GetSnapshotAsync();
                    return snapshot;
                }) //Get the snapshot
                .SelectMany(r => r.Result.Documents.Select(doc => doc.ConvertTo<Place>())) //Resolve the establishments
                .ToHashSet(); //Erase duplicates



            var result = uniquePlaces.Where(p => DistanceService.Distance(p.Location, center) < distance).ToList();
            return result;
        }

        [HttpGet("recommended")]
        public async Task<IEnumerable<Place>> GetRecommendedEstablishmentsForPlaylist(string playlistId, double lat, double lon, double distance)
        {
            string userId = HttpContext.User.Identity.Name;
            var userRef = _db.Collection("users").Document(userId);

            //Get his spotify token

            //Get playlist songs from spotify

            //Match with storedSongs in Db for tags

            //Call lastFm for the rest, if empty save with flag to not ask for it

            //Update tags

            //Algorithm magic (Get every place surrounding that, apply the algorithm for those)

            throw new NotImplementedException();
        }

        [HttpGet("recommended/top")]
        public async Task<IEnumerable<Place>> GetRecommendedEstablishmentsForTop(double lat, double lon, double distance)
        {
            string userId = HttpContext.User.Identity.Name;
            var userRef = _db.Collection("users").Document(userId);

            //Get his spotify token

            //Get playlists songs from spotify

            //Match with storedSongs in Db for tags

            //Call lastFm for the rest, if empty save with flag to not ask for it

            //Update tags

            //Algorithm magic (Get every place surrounding that, apply the algorithm for those)


            throw new NotImplementedException();

        }

        [HttpPost]
        public async Task<Place> CreateEstablishment([FromBody] Place establishment)
        {
            var userId = HttpContext.User.Identity.Name;

            var docRef = PlaceService.CreatePlace(establishment);

            //TODO
            throw new NotImplementedException();
        }

        [HttpPatch]
        public async Task<Place> UpdateEstablishment(Place establishment)
        {

            throw new NotImplementedException();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEstablishment(string establishmentId)
        {

            throw new NotImplementedException();
        }

        [HttpPost("{establishmentId}/song")]
        public async Task<IActionResult> RecognizeSong(string establishmentId, Song song)
        {
            var songRef = _db.Collection("places").Document($"{establishmentId}").Collection("recognizedMusic").Document($"{song.Id}");

            var snapshot = await songRef.GetSnapshotAsync();

            //Update

            throw new NotImplementedException();

            return Ok();
        }

        [HttpPost("{placeId}/rate")]
        public async Task<IActionResult> RatePlace(string placeId, double value)
        {
            //Get current user

            //Access places/{placeId}/ratings/{userId}

            //Create or updateValue

            throw new NotImplementedException();
        }


    }
}