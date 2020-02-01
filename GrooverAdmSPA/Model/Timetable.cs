using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    [FirestoreData(ConverterType = typeof(FirestoreEntityConverter<Timetable>))]
    public class Timetable : FirestoreEntity<Timetable>
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public List<Schedule> Schedules { get; set; }
        [FirestoreProperty]
        public DayOfWeek Day { get; set; }
        [FirestoreProperty(ConverterType = typeof(FirestoreEntityConverter<Playlist>))]
        public Playlist Playlist { get; set; }
    }
}
