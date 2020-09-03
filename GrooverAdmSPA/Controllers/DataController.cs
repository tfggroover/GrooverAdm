using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GrooverAdmSPA.Model;
using Microsoft.AspNetCore.Mvc;

namespace GrooverAdmSPA.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly FirestoreDb _db;

        public DataController(FirestoreDb db)
        {
            _db = db;
        }


        [HttpGet("test")]
        public async Task<WriteResult> Test()
        {


            var result = await _db.Collection("Test").Document("test").SetAsync(new Place
            {
                Address = "asdasd",
                Recognized = new Dictionary<string, int>
                {
                    {"lelelelele", 1 }
                },
                Timetables = new List<Timetable>
                {
                    new Timetable
                    {
                        Id = "adsasdsa",
                        Day = DayOfWeek.Monday,
                        Schedules = new List<Schedule>
                        {
                            new Schedule
                            {
                                Start = DateTime.Now,
                                End = DateTime.Now.AddHours(3)
                            }
                        }
                        
                    }
                }
            }) ;

            return result;
        }


    }
}
