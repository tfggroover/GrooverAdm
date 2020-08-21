using GrooverAdm.Business.Services.Places;
using GrooverAdm.Entities.Application;
using GrooverAdm.Entities.LastFm;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services
{
    public class RecommendationService
    {
        private readonly SpotifyService _spotify;
        private readonly LastFmService lastFmService;
        private readonly IConfiguration Configuration;

        public RecommendationService(SpotifyService spotify, LastFmService lastFmService, IConfiguration configuration)
        {
            _spotify = spotify;
            this.lastFmService = lastFmService;
            Configuration = configuration;
        }


        public async Task<IEnumerable<ComparedPlace>> GetSimilarPlaylistsOrdered(Entities.Application.Playlist compared, List<Place> places, string spotifyToken)
        {
            var client = new HttpClient();


            OccurrenceSearcher occurrences = await FindPlaylistOccurences(compared, places, spotifyToken, client);

            var ponderedTags = PonderOccurences(occurrences.Tags);
            var ponderedGenres = PonderOccurences(occurrences.Genres);

            var fmSucc = double.TryParse(Configuration["FmPonderation"], out var fmCoeff);
            var spoSucc = double.TryParse(Configuration["SpoPonderation"], out var spoCoeff);
            // ordenar según getSimilitud según la ponderación que se quiera
            var result = places.Select(p =>
            {
                var similitude = GetSimilitude(ponderedTags[compared.Id], ponderedTags[p.MainPlaylist.Id]) * (fmSucc ? fmCoeff : 0.5)
                    + GetSimilitude(ponderedGenres[compared.Id], ponderedGenres[p.MainPlaylist.Id]) * (spoSucc ? spoCoeff : 0.5);
                var res = new ComparedPlace (p);
                res.Similitude = similitude;
                return res;
            }).OrderByDescending(c => c.Similitude);

            return result;
        }

        private static Dictionary<string, Dictionary<string, double>> PonderOccurences(ConcurrentDictionary<string, Dictionary<string, int>> occurrences)
        {
            var tags = new Dictionary<string, Dictionary<string, int>>(occurrences);
            var ponderedTags = new Dictionary<string, Dictionary<string, double>>();
            var tagFm = new Dictionary<string, int>();
            var sumTagsLastFm = 0;
            foreach (var p in tags)
            {
                ponderedTags.Add(p.Key, new Dictionary<string, double>());
                foreach (var t in p.Value)
                {
                    sumTagsLastFm += t.Value;
                    if (tagFm.ContainsKey(t.Key))
                        tagFm[t.Key]++;
                    else
                        tagFm[t.Key] = 1;
                }
            }

            foreach (var t in tagFm)
            {
                var idf = 1 + Math.Log(tags.Count / t.Value);
                foreach (var p in ponderedTags)
                {
                    if (tags.ContainsKey(p.Key) && tags[p.Key].ContainsKey(t.Key))
                        p.Value[t.Key] = idf * tags[p.Key][t.Key];
                    else
                        p.Value[t.Key] = 0;
                }
            }

            return ponderedTags;
        }

        private async Task<OccurrenceSearcher> FindPlaylistOccurences(Entities.Application.Playlist compared, List<Place> places, string spotifyToken, HttpClient client)
        {
            var occurrences = new OccurrenceSearcher();

            await places.ToAsyncEnumerable().ForEachAsync( async (Place place) =>
            {
                var header = await _spotify.GetPlaylistHeader(client, spotifyToken, place.MainPlaylist.Id);
                if (header != null)
                {
                    if (header.Snapshot_id == place.MainPlaylist.SnapshotVersion && place.MainPlaylist.Tags != null && place.MainPlaylist.Tags.Any() && place.MainPlaylist.Genres != null && place.MainPlaylist.Genres.Any())
                    {
                        occurrences.UpdateTags(place.MainPlaylist.Tags, place.MainPlaylist.Id);
                        occurrences.UpdateGenres(place.MainPlaylist.Genres, place.MainPlaylist.Id);
                    }
                    else
                    {
                        var playlistSongs = (await _spotify.GetSongsFromPlaylist(client, spotifyToken, place.MainPlaylist.Id)).Items.Select(s => new Entities.Application.Song(s)).ToList();

                        await GetTagsAndGenresFromSongs(spotifyToken, place.MainPlaylist.Id, playlistSongs, occurrences);

                        place.MainPlaylist.SnapshotVersion = header.Snapshot_id;
                        place.MainPlaylist.Songs = playlistSongs;
                        place.MainPlaylist.Tags = occurrences.GetTags(place.MainPlaylist.Id);
                        place.MainPlaylist.Genres = occurrences.GetGenres(place.MainPlaylist.Id);
                    }
                }
            });

            await GetTagsAndGenresFromSongs(spotifyToken, compared.Id, compared.Songs, occurrences);
            compared.Tags = occurrences.GetTags(compared.Id);
            compared.Genres = occurrences.GetGenres(compared.Id);

            return occurrences;
        }

        private double GetSimilitude(Dictionary<string, double> first, Dictionary<string, double> second)
        {
            var top = 0.0;
            var botl = 0.0;
            var botr = 0.0;

            foreach (var t in first)
            {
                var secondVal = 0.0;
                if (second.ContainsKey(t.Key))
                    secondVal = second[t.Key];
                top += secondVal * t.Value;
                botl += Math.Pow(t.Value, 2);
                botr += Math.Pow(secondVal, 2);
            }

            var similitude = top / (Math.Sqrt(botl) * Math.Sqrt(botr));
            return similitude;
        }

        private async Task GetTagsAndGenresFromSongs(string token, string playlistId, List<Entities.Application.Song> songs, OccurrenceSearcher occurrences)
        {
            var client = new HttpClient();
            var lastFmTagOccurrenceConc = new ConcurrentDictionary<string, int>();
            var spotifyGenreOccurrenceConc = new ConcurrentDictionary<string, int>();
            await songs.ToAsyncEnumerable().ForEachAsync(async (song) =>
            {
                var fmSuccess = false;
                while (!fmSuccess)
                {
                    try
                    {
                        var lastFmTags = await lastFmService.GetTrackTags(client, song.Name, song.Artists[0]?.Name);
                        if (lastFmTags != null && lastFmTags.Toptags != null && lastFmTags.Toptags.Tag != null)
                        {
                            lastFmTags.Toptags.Tag.Sort(new TagComparer());

                            lastFmTags.Toptags.Tag.Take(5).ToList().ForEach(t =>
                            {
                                lastFmTagOccurrenceConc.AddOrUpdate(t.Name, 1, (k, old) => old + 1);
                            });
                        }
                        fmSuccess = true;
                    }
                    catch (TaskCanceledException) { }
                }
                var artists = song.Artists.Select(a => a.Id).ToList();
                if (artists.Any())
                {
                    var newArtists = artists.Where(a => !occurrences.ArtistExists(a)).ToList();

                    if (newArtists.Any())
                    {
                        newArtists.ForEach(a => occurrences.UpdateArtistGenres(new List<string>(), a));
                        var stringArtists = newArtists.Aggregate((a, b) => a + "," + b);
                        var spotifySuccess = false;
                        while (!spotifySuccess)
                        {
                            try
                            {
                                var spotifyTags = await _spotify.GetArtists(client, token, stringArtists);
                                if (spotifyTags != null)
                                {
                                    spotifyTags.ForEach(a => occurrences.UpdateArtistGenres(a.Genres.ToList(), a.Id));
                                    spotifySuccess = true;
                                }
                            }
                            catch (TaskCanceledException) { }
                        }
                    }

                    artists.ForEach(s =>
                    {
                        occurrences.GetArtistGenres(s).ForEach(g =>
                        {
                        // Add Genre occurrence
                        spotifyGenreOccurrenceConc.AddOrUpdate(g, 1, (k, l) => l + 1);
                        });
                    });
                }
            });

            occurrences.UpdateTags(new Dictionary<string, int>(lastFmTagOccurrenceConc), playlistId);
            occurrences.UpdateGenres(new Dictionary<string, int>(spotifyGenreOccurrenceConc), playlistId);
        }

    }

    public class OccurrenceSearcher
    {
        public ConcurrentDictionary<string, Dictionary<string, int>> Tags = new ConcurrentDictionary<string, Dictionary<string, int>>();
        public ConcurrentDictionary<string, Dictionary<string, int>> Genres = new ConcurrentDictionary<string, Dictionary<string, int>>();
        public ConcurrentDictionary<string, List<string>> Artists = new ConcurrentDictionary<string, List<string>>();


        public Dictionary<string, int> GetTags(string playlist)
        {
            return Tags.GetValueOrDefault(playlist);
        }
        public void UpdateTags(Dictionary<string, int> tags, string playlist)
        {
            Tags.AddOrUpdate(playlist, tags, (k, l) => tags);
        }

        public Dictionary<string, int> GetGenres(string playlist)
        {
            return Genres.GetValueOrDefault(playlist);
        }
        public void UpdateGenres(Dictionary<string, int> genres, string playlist)
        {
            Genres.AddOrUpdate(playlist, genres, (k, l) => genres);
        }
        public List<string> GetArtistGenres(string artistId)
        {
            return Artists.GetValueOrDefault(artistId);
        }
        public bool ArtistExists(string artistId)
        {
            return Artists.ContainsKey(artistId);
        }
        public void UpdateArtistGenres(List<string> genres, string artistId)
        {
            Artists.AddOrUpdate(artistId, genres, (k, l) => genres);
        }
    }
}
