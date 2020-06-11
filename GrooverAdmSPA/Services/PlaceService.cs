using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GrooverAdmSPA.Model;

namespace GrooverAdmSPA.Services
{
    public class PlaceService
    {
        private readonly FirestoreDb _db;
        public PlaceService(FirestoreDb db)
        {
            _db = db;
        }
        internal static object CreatePlace(Place establishment)
        {


            throw new NotImplementedException();
        }
    }
}
