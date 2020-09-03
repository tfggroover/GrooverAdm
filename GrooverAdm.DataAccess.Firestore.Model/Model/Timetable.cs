using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class Timetable
    {
        [FirestoreProperty]
        public List<Schedule> Schedules { get; set; }
        [FirestoreProperty]
        public DayOfWeek Day { get; set; }
    }
}
