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
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<IEnumerable<Place>>> GetEstablishments(double lat, double lon, double distance, int page = 1, int pageSize = 25)
        {
            var location = new Geolocation
            {
                Latitude = lat,
                Longitude = lon
            };
            if (page < 1)
                return BadRequest();
            var result = await _placesService.GetPlaces(page, pageSize, location, distance, true);

            return Ok(result);
        }
        /// <summary>
        /// Obtiene los lugares recomendados en funcion de la playlist enviada
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="distance"></param>
        /// <param name="spoToken">Token de spotify</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/place/recommended")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ComparedPlace>>> GetRecommendedEstablishmentsForPlaylist(string playlistId, double lat, double lon, double distance, string spoToken, int page = 1, int pageSize = 25)
        {
            //string userId = GetUserId();
            var client = new HttpClient();

            //Get his spotify token
            //var user = await _userService.GetUser(userId);

            var token = spoToken;
            //Get playlist songs from spotify

            var spotiPlaylist = await _spotify.GetPlaylist(client, token, playlistId);
            if (spotiPlaylist == null)
                return Unauthorized("Please refresh your spotify token");
            var playlist = new GrooverAdm.Entities.Application.Playlist
            {
                Id = playlistId,
                Name = spotiPlaylist.Name,
                SnapshotVersion = spotiPlaylist.Snapshot_id,
                ImageUrl = spotiPlaylist.Images[0]?.Url,
                Songs = spotiPlaylist.Tracks.Items.Select(s =>
                    new GrooverAdm.Entities.Application.Song
                    {
                        Artists = s.Track.Artists.Select(a => new GrooverAdm.Entities.Application.Artist { Id = a.Id, Name = a.Name }).ToList(),
                        Id = s.Track.Id,
                        Name = s.Track.Name
                    }).ToList()
            };

            //Match with storedSongs in Db for tags
            var places = await _placesService.GetRecommendedPlaces(page, pageSize, new Geolocation(lat, lon), distance, playlist, token);

            return Ok(places);
        }


        /// <summary>
        /// Obtiene los lugares recomendados en funcion del top50 del usuario
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="distance"></param>
        /// <param name="spoToken">Token de spotify</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/place/recommended/top")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ComparedPlace>>> GetRecommendedEstablishmentsForTop(double lat, double lon, double distance, string spoToken, int page = 1, int pageSize = 25)
        {
            //string userId = GetUserId();
            var client = new HttpClient();

            //Get his spotify token
            //var user = await _userService.GetUser(userId);

            var token = spoToken;
            //Get playlist songs from spotify

            var spotiPlaylist = await _spotify.GetUsersTopTracks(client, token);
            if (spotiPlaylist == null)
                return Unauthorized();
            var playlist = new GrooverAdm.Entities.Application.Playlist
            {
                Id = "top",
                Name = "Top",
                SnapshotVersion = "-",
                ImageUrl = "",
                Songs = spotiPlaylist.Items.Select(s =>
                    new GrooverAdm.Entities.Application.Song
                    {
                        Artists = s.Artists.Select(a => new GrooverAdm.Entities.Application.Artist { Id = a.Id, Name = a.Name }).ToList(),
                        Id = s.Id,
                        Name = s.Name
                    }).ToList()
            };

            //Match with storedSongs in Db for tags
            var places = await _placesService.GetRecommendedPlaces(page, pageSize, new Geolocation(lat, lon), distance, playlist, token);

            return Ok(places);
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

            var docRef = await _placesService.CreatePlace(establishment);

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
        /// We gucci
        /// </summary>
        /// <param name="establishmentId"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/place/{establishmentId}/song")]
        public async Task<IActionResult> RecognizeSong(string establishmentId, [FromBody] GrooverAdm.Entities.Application.Song song)
        {

            var res = await _placesService.RecognizeSong(establishmentId, song, GetUserId());

            if (res)
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