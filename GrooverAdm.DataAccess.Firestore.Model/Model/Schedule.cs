using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class Schedule
    {
        [FirestoreProperty]
        public DateTime Start { get; set; }

        [FirestoreProperty]
        public DateTime End { get; set; }
    }
}
