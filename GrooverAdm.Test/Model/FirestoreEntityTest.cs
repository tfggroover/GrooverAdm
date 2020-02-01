using GrooverAdmSPA.Model;
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

            var place = new Establishment()
            {
                Address = "Test",
                DisplayName = "Testeroni",
                Geohash = NGeoHash.GeoHash.Encode(37.3738584, -5.968221),
                Id = Guid.NewGuid().ToString(),
                Location = new Google.Cloud.Firestore.GeoPoint(37.3738584, -5.968221),
                Owners = new List<User>
                {
                    new User
                    {
                        Born = 2000,
                        DisplayName = "Pepito",
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
                Playlist = new Playlist
                {
                    Hash = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid().ToString(),
                    Songs = new List<Song>
                    {
                        new Song
                        {
                            Id = Guid.NewGuid().ToString(),
                            Tags = new List<string>
                            {
                                "pop", "rock"
                            },
                            Data = new
                            {
                                Description = "Utter garbage"
                            }
                        }
                    },
                    Metrics = new
                    {
                        Tempo = "Pretty fast"
                    }

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
                Recognized = new Dictionary<string, int>
                {
                    { Guid.NewGuid().ToString() , 1 }
                },
                Timetables = new List<Timetable>()
                {
                    new Timetable
                    {
                        Day = DayOfWeek.Monday,
                        Id = Guid.NewGuid().ToString(),
                        Playlist = new Playlist
                        {
                            Hash = Guid.NewGuid().ToString(),
                            Id = Guid.NewGuid().ToString(),

                        },
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
                        Id = Guid.NewGuid().ToString(),
                        Playlist = new Playlist
                        {
                            Hash = Guid.NewGuid().ToString(),
                            Id = Guid.NewGuid().ToString(),

                        },
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

            var expected = new Dictionary<string, object>
            {
                { "Address", "Test" },
                { "DisplayName", "Testeroni"},
                { "Geohash", NGeoHash.GeoHash.Encode(37.3738584, -5.968221) },
                { "Id", Guid.NewGuid().ToString() },
                { "Location", new Dictionary<string, object>
                    {
                        { "Latitude", 37.3738584 },
                        { "Longitude", -5.968221 }
                    }
                },
                { "Owners" , new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "Born" , 2000 },
                            { "DisplayName", "Pepito" },
                            { "Id", Guid.NewGuid().ToString() }
                        },
                        new Dictionary<string, object>
                        {
                            { "Born", 2000 },
                            { "DisplayName", "Pepito2" },
                            { "Id", Guid.NewGuid().ToString() }
                        },
                    }
                },
                { "Phone", "55555555" },
                { "Playlist", new Dictionary<string, object>
                    {
                        { "Hash" , Guid.NewGuid().ToString() },
                        { "Id", Guid.NewGuid().ToString() },
                        { "Songs", new List<Dictionary<string,object>>
                            {
                                new Dictionary<string, object>
                                {
                                    {"Id", Guid.NewGuid().ToString() },
                                    { "Tags" , new List<string>
                                        {
                                            "pop", "rock"
                                        }
                                    },
                                    { "Data", new Dictionary<string, object>
                                        {
                                            { "Description", "Utter garbage" }
                                        }
                                    }
                                }
                            }
                        },
                        { "Metrics", new Dictionary<string, object>
                            {
                                { "Tempo", "Pretty fast" }
                            }
                        }

                    }
                },
                { "Ratings", new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "Id", Guid.NewGuid().ToString() },
                            { "User", new Dictionary<string, object>
                                {
                                    { "Born", 2000 },
                                    { "DisplayName", "NotPepito" },
                                    { "Id", Guid.NewGuid().ToString() }
                                }

                            }
                        }
                    }
                },
                { "Recognized", new Dictionary<string, int>
                    {
                        { Guid.NewGuid().ToString() , 1 }
                    }
                },
                { "Timetables", new List<Dictionary<string, object>>()
                    {
                        new Dictionary<string, object>
                        {
                            { "Day", DayOfWeek.Monday },
                            { "Id", Guid.NewGuid().ToString() },
                            { "Playlist", new Dictionary<string, object>
                                {
                                    { "Hash", Guid.NewGuid().ToString() },
                                    { "Id", Guid.NewGuid().ToString() },

                                }
                            },
                            { "Schedules", new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        { "Start", now },
                                        { "End", notNow }
                                    }
                                }
                            }
                        },
                        new Dictionary<string, object>
                        {
                            { "Day", DayOfWeek.Tuesday },
                            { "Id", Guid.NewGuid().ToString() },
                            { "Playlist", new Dictionary<string, object>
                                {
                                    { "Hash", Guid.NewGuid().ToString() },
                                    { "Id", Guid.NewGuid().ToString() }
                                }
                            },
                            { "Schedules", new List<Dictionary<string, object>>
                                {
                                    new Dictionary<string, object>
                                    {
                                        { "Start", now },
                                        { "End", notNow }
                                    }
                                }
                            }

                        }
                    }
                }
            };

            var result = place.ToDictionary();


            Assert.AreEqual(result, expected);
        }


    }
}
