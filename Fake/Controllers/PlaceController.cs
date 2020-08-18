using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GrooverAdm.Business.Services;
using GrooverAdm.Business.Services.Places;
using GrooverAdm.Business.Services.Playlist;
using GrooverAdm.Business.Services.User;
using GrooverAdm.Entities.Application;
using GrooverAdm.Entities.LastFm;
using GrooverAdm.Entities.Spotify;
using GrooverAdmSPA.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Fake.Controllers
{
    //[Authorize]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly FirestoreDb _db;
        private readonly IPlacesService _placesService;
        private readonly IUserService _userService;
        private readonly SpotifyService _spotify;
        private readonly LastFmService _lastFm;
        private readonly IConfiguration Configuration;
        

        public PlaceController(FirestoreDb db, IPlacesService placesService, IUserService userService, SpotifyService spotify, LastFmService lastFm, IConfiguration config)
        {
            _db = db;
            _placesService = placesService;
            _userService = userService;
            _spotify = spotify;
            _lastFm = lastFm;
            Configuration = config;
        }

        /// <summary>
        /// Retrieves the establishments surrounding [<paramref name="lat"/>, <paramref name="lon"/>] in 
        /// the distance provided
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="distance">Distance in METERS</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/place")]
        public async Task<IEnumerable<Place>> GetEstablishments(double lat, double lon, double distance, int page = 0, int pageSize = 25)
        {
            var location = new Geolocation
            {
                Latitude = lat,
                Longitude = lon
            };
            var result = await _placesService.GetPlaces(page, pageSize, location, distance, true);

            return result;
        }
        /// <summary>
        /// HACE MUCHA PARAFERNALIA EN MEDIO PERO DE MOMENTO NO RECOMIENDA, SON LAS 3 DE LA MAÑANA HELP
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="distance"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/place/recommended")]
        public async Task<IEnumerable<ComparedPlace>> GetRecommendedEstablishmentsForPlaylist(string playlistId, double lat, double lon, double distance, int page = 1, int pageSize = 25)
        {
            string userId = GetUserId();
            var client = new HttpClient();

            //Get his spotify token
            var user = await _userService.GetUser(userId);

            var token = user.CurrentToken;
            //Get playlist songs from spotify

            var songs = await _spotify.GetSongsFromPlaylist(client, token, playlistId);
            //Match with storedSongs in Db for tags
            var places = await _placesService.GetPlaces(1, int.MaxValue, new Geolocation { Latitude = lat, Longitude = lon }, distance, true);

            /*


            var playlistVectorsFm = new Dictionary<string, Dictionary<string, double>>();
            var playlistVectorsSpoti = new Dictionary<string, Dictionary<string, double>>();

            var playlistFMOccurrenceDictionary = new ConcurrentDictionary<string, Dictionary<string, int>>();
            var playlistGenreOccurenceDictionary = new ConcurrentDictionary<string, Dictionary<string, int>>();
            var artistsChecked = new ConcurrentDictionary<string, List<string>>();

            Parallel.ForEach(places, async (Place place) =>
            {
                playlistVectorsSpoti[place.MainPlaylist.Id] = new Dictionary<string, double>();
                playlistVectorsFm[place.MainPlaylist.Id] = new Dictionary<string, double>();
                if (place.MainPlaylist.Tags.Any() && place.MainPlaylist.Genres.Any())
                {
                    playlistFMOccurrenceDictionary.AddOrUpdate(place.MainPlaylist.Id, place.MainPlaylist.Tags, (k, l) => place.MainPlaylist.Tags);
                    playlistGenreOccurenceDictionary.AddOrUpdate(place.MainPlaylist.Id, place.MainPlaylist.Genres, (k, l) => place.MainPlaylist.Genres);
                }
                else
                {
                    var playlistSongs = await _spotify.GetSongsFromPlaylist(client, token, playlistId);
                    
                    GetTagsAndGenresFromSongs(client, token, playlistSongs, artistsChecked, out var lastFmTagOccurrenceInner, out var spotifyGenreOccurrenceInner);

                    playlistFMOccurrenceDictionary.AddOrUpdate(place.MainPlaylist.Id, lastFmTagOccurrenceInner, (k, l) => lastFmTagOccurrenceInner);
                    playlistGenreOccurenceDictionary.AddOrUpdate(place.MainPlaylist.Id, spotifyGenreOccurrenceInner, (k, l) => spotifyGenreOccurrenceInner);

                    place.MainPlaylist.Songs = playlistSongs.Items.Select(s => new GrooverAdm.Entities.Application.Song(s)).ToList();
                    //UpdatePlaylist with songs, tags and genres
                    await _placesService.UpdatePlaylist(place, lastFmTagOccurrenceInner, spotifyGenreOccurrenceInner);
                    
                }
            });


            GetTagsAndGenresFromSongs(client, token, songs, artistsChecked, out var lastFmTagOccurrence, out var spotifyGenreOccurrence);
            playlistFMOccurrenceDictionary.AddOrUpdate(playlistId, lastFmTagOccurrence, (k, l) => lastFmTagOccurrence);
            playlistGenreOccurenceDictionary.AddOrUpdate(playlistId, spotifyGenreOccurrence, (k, l) => spotifyGenreOccurrence);

            var playFMO = new Dictionary<string, Dictionary<string, int>>(playlistFMOccurrenceDictionary);
            var playGenre = new Dictionary<string, Dictionary<string, int>>(playlistGenreOccurenceDictionary);
            var sumTagsLastFm = 0;
            var sumTagsSpoti = 0;
            var tagSpoti = new Dictionary<string, int>();
            var tagFm = new Dictionary<string, int>();

            foreach (var p in playFMO)
            {
                foreach (var t in p.Value)
                {
                    sumTagsLastFm += t.Value;
                    if (tagFm.ContainsKey(t.Key))
                        tagFm[t.Key]++;
                    else
                        tagFm[t.Key] = 1;
                }
            }

            foreach (var p in playGenre)
            {
                foreach (var t in p.Value)
                {
                    sumTagsSpoti += t.Value;
                    if (tagSpoti.ContainsKey(t.Key))
                        tagSpoti[t.Key]++;
                    else
                        tagSpoti[t.Key] = 1;
                }
            }

            foreach (var t in tagFm)
            {
                var idf = Math.Log(playFMO.Count / t.Value);
                foreach(var p in playlistVectorsFm)
                {
                    if (playFMO.ContainsKey(p.Key) && playFMO[p.Key].ContainsKey(t.Key))
                        p.Value[t.Key] = idf * playFMO[p.Key][t.Key];
                    else
                        p.Value[t.Key] = 0;
                }
            }


            foreach (var t in tagSpoti)
            {
                var idf = Math.Log(playGenre.Count / t.Value);
                foreach (var p in playlistVectorsSpoti)
                {
                    if (playGenre.ContainsKey(p.Key) && playGenre[p.Key].ContainsKey(t.Key))
                        p.Value[t.Key] = idf * playGenre[p.Key][t.Key];
                    else
                        p.Value[t.Key] = 0;
                }
            }

            var fmSucc = double.TryParse(Configuration["FmPonderation"], out var fmCoeff);
            var spoSucc = double.TryParse(Configuration["SpoPonderation"], out var spoCoeff);
            // ordenar según getSimilitud según la ponderación que se quiera
            var result = places.Select(p =>
            {
                var similitude = GetSimilitude(playlistVectorsFm[playlistId], playlistVectorsFm[p.MainPlaylist.Id]) * (fmSucc? fmCoeff : 0.5) + GetSimilitude(playlistVectorsSpoti[playlistId], playlistVectorsSpoti[p.MainPlaylist.Id]) * (spoSucc?spoCoeff : 0.5);
                var res = (ComparedPlace) p;
                res.Similitude = similitude;
                return res;
            }).OrderByDescending(c => c.Similitude).Skip((page - 1) * pageSize).Take(pageSize);
            */
            return null;
        }



        [HttpGet]
        [Route("api/place/recommended/top")]
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
        /// <summary>
        /// READY TO GO? NOT TESTED
        /// </summary>
        /// <param name="establishment"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/place")]
        public async Task<Place> CreateEstablishment([FromBody] Place establishment)
        {
            var userId = HttpContext.User.Identity.Name;

            var docRef = await  _placesService.CreatePlace(establishment);

            return docRef;
        }

        [HttpPatch]
        [Route("api/place")]
        public async Task<Place> UpdateEstablishment(Place establishment)
        {

            var docRef = await _placesService.UpdatePlace(establishment);

            return docRef;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="establishmentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/place")]
        public async Task<IActionResult> DeleteEstablishment(string establishmentId)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="establishmentId"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/place/{establishmentId}/song")]
        public async Task<IActionResult> RecognizeSong(string establishmentId, [FromBody] GrooverAdm.Entities.Application.Song song)
        {

            var res = await _placesService.RecognizeSong(establishmentId, song);

            if(res)
                return Ok();

            return BadRequest();
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="placeId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/place/{placeId}/rate")]
        public async Task<IActionResult> RatePlace(string placeId, double value)
        {
            //Get current user

            //Access places/{placeId}/ratings/{userId}

            //Create or updateValue

            throw new NotImplementedException();
        }


    }
}