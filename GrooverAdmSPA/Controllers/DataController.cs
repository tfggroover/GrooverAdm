using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
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


        [HttpGet("[action]")]
        public async Task<IEnumerable<Dictionary<string, object>>> DistanceToEstablishments(double lat, double lon, double distance)
        {
            var variance = distance / 110000;
            
            var places = _db.Collection("places")
                    .WhereGreaterThanOrEqualTo("location", new GeoPoint(lat - variance, lon - variance))
                    .WhereLessThanOrEqualTo("location", new GeoPoint(lat + variance, lon + variance))
                    .StreamAsync();
            var result = await  places.Select(p => p.ToDictionary()).ToList();
            return result;
        }


    }
}
