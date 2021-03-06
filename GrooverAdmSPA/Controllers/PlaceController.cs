﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GrooverAdm.Business.Services;
using GrooverAdm.Business.Services.Places;
using GrooverAdm.Business.Services.User;
using GrooverAdm.Entities.Application;
using GrooverAdm.Entities.LastFm;
using GrooverAdmSPA.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooverAdmSPA.Controllers
{
    //[Route("api/[controller]")]
    //[Authorize()]
    [ApiController]
    public class PlaceController : Controller
    {
        private readonly FirestoreDb _db;
        private readonly IPlacesService _placesService;
        private readonly IUserService _userService;
        private readonly SpotifyService _spotify;
        private readonly LastFmService _lastFm;

        public PlaceController(FirestoreDb db, IPlacesService placesService, IUserService userService, SpotifyService spotify, LastFmService lastFm)
        {
            _db = db;
            _placesService = placesService;
            _userService = userService;
            _spotify = spotify;
            _lastFm = lastFm;
        }

        [HttpGet]
        [Route("api/place")]
        public async Task<IEnumerable<Place>> GetEstablishments(double lat, double lon, double distance)
        {
            var location = new Geolocation
            {
                Latitude = lat,
                Longitude = lon
            };
            var result = await _placesService.GetPlaces(0, 25, location, distance,  true);
            
            return result;
        }

        [HttpGet("recommended")]
        [Route("api/place/recommended")]
        public async Task<IEnumerable<Place>> GetRecommendedEstablishmentsForPlaylist(string playlistId, double lat, double lon, double distance)
        {
            string userId = HttpContext.User.Identity.Name;
            var client = new HttpClient();

            //Get his spotify token
            var user = await _userService.GetUser(userId);

            var token = user.CurrentToken;
            //Get playlist songs from spotify

            var songs = await _spotify.GetSongsFromPlaylist(client, token, playlistId);
            //Match with storedSongs in Db for tags


            var lastFmTagOccurrence = new ConcurrentDictionary<string, int>();
            var spotifyGenreOccurrence = new ConcurrentDictionary<string, int>();
            var artistsChecked = new ConcurrentDictionary<string, List<string>>();
            //Call lastFm for the rest, if empty save with flag to not ask for it
            Parallel.ForEach(songs.Items, async (song) =>
            {
                var lastFmTags = await _lastFm.GetTrackTags(client, song.Track.Name, song.Track.Artists[0]?.Name);
                lastFmTags.Toptags.Tag.Sort(new TagComparer());

                lastFmTags.Toptags.Tag.Take(5).ToList().ForEach(t =>
                {
                    lastFmTagOccurrence.AddOrUpdate(t.Name, 1, (k, i) => i++);
                });

                var artists = song.Track.Artists.Select(a => a.Id)
                        .Where(a => !artistsChecked.ContainsKey(a));
                artists.ToList().ForEach(a => artistsChecked.AddOrUpdate(a, new List<string>(), (k, l) => new List<string>()));


                var stringArtists = artists.Aggregate((a, b) => a + "," + b);
                var spotifyTags = await _spotify.GetArtists(client, token, stringArtists);
                spotifyTags.ForEach(a =>
                {
                    artistsChecked.AddOrUpdate(a.Id, a.Genres.ToList(), (k, l) => a.Genres.ToList());
                    a.Genres.ToList().ForEach(g =>
                    {
                        // Add Genre occurrence
                        spotifyGenreOccurrence.AddOrUpdate(g, 1, (k, l) => l++);
                    });
                });
            });

            //Update tags
            
            //Algorithm magic (Get every place surrounding that, apply the algorithm for those)
            //With today's playlist
            var places = _placesService.GetPlaces(0, 25, new Geolocation { Latitude = lat, Longitude = lon }, distance, true);

            throw new NotImplementedException();
        }

        [HttpGet("recommended/top")]
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

        [HttpPost]
        [Route("api/place")]
        public async Task<Place> CreateEstablishment([FromBody] Place establishment)
        {
            var userId = HttpContext.User.Identity.Name;

            var docRef = await  _placesService.CreatePlace(establishment, userId);

            return docRef;
        }

        [HttpPatch]
        [Route("api/place")]
        public async Task<Place> UpdateEstablishment(Place establishment)
        {

            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("api/place")]
        public async Task<IActionResult> DeleteEstablishment(string establishmentId)
        {

            throw new NotImplementedException();
        }

        [HttpPost("{establishmentId}/song")]
        [Route("api/place/{establishmentId}/song")]
        public async Task<IActionResult> RecognizeSong(string establishmentId, Song song)
        {
            var songRef = _db.Collection("places").Document($"{establishmentId}").Collection("recognizedMusic").Document($"{song.Id}");

            var snapshot = await songRef.GetSnapshotAsync();

            //Update

            throw new NotImplementedException();

            return Ok();
        }

        [HttpPost("{placeId}/rate")]
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