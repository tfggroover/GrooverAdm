using GrooverAdm.Entities.Application;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Test.Model
{
    public class FirestoreEntityTest
    {
        [SetUp]
        public void Setup()
        {

        }


        [TestCase()]
        public void ToDictionaryTest()
        {
            var now = DateTime.Now;
            var notNow = now.AddHours(3);

            var play = new Playlist
            {
                Id = Guid.NewGuid().ToString(),
                Url = "La url de la lista hacia spotify en caso de que quieras permitir al usuario abrirla",
                Songs = new List<Song>
                {
                    new Song
                    {
                        Id = Guid.NewGuid().ToString()
                    }
                },
                SnapshotVersion = "v2"
            };

            var place = new Place()
            {
                Address = "Test",
                DisplayName = "Testeroni",
                Geohash = NGeoHash.GeoHash.Encode(37.3738584, -5.968221),
                Id = Guid.NewGuid().ToString(),
                Location = new Geolocation { Latitude = 37.3738584, Longitude = -5.968221 },
                Owners = new List<User>
                {
                    new User
                    {
                        Born = 2000,
                        DisplayName = "En esta lista de usuarios también puedo devolverte solo el Id de referencia, lo que prefieras, si vas a mostrar algo del dueño evidentemente te lo doy relleno",
                        Id = Guid.NewGuid().ToString()
                    },
                    new User
                    {
                        Born = 2000,
                        DisplayName = "Pepito2",
                        Id = Guid.NewGuid().ToString()
                    },
                },
                Phone = "55555555",
                MainPlaylist = play,
                WeeklyPlaylists = new Dictionary<string, Playlist>
                {
                    { "Monday",  play },
                    { "Tuesday", play }
                },
                Ratings = new List<Rating>
                {
                    new Rating
                    {
                        Id = Guid.NewGuid().ToString(),
                        User = new User
                        {
                            Born = 2000,
                            DisplayName = "NotPepito",
                            Id = Guid.NewGuid().ToString()
                        }
                    }
                },
                Timetables = new List<Timetable>()
                {
                    new Timetable
                    {
                        Day = DayOfWeek.Monday,
                        Schedules = new List<Schedule>
                        {
                            new Schedule
                            {
                                Start = now,
                                End = notNow
                            }
                        }
                    },
                    new Timetable
                    {
                        Day = DayOfWeek.Tuesday,
                        Schedules = new List<Schedule>
                        {
                            new Schedule
                            {
                                Start = now,
                                End = notNow
                            }
                        }
                    }
                }
            };

            var pla = JsonConvert.SerializeObject(place);

        }


    }
}
