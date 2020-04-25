using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public class Schedule : FirestoreEntity<Schedule>
    {
        [FirestoreProperty]
        public DateTime Start { get; set; }
        [FirestoreProperty]
        public DateTime End { get; set; }
    }
}
