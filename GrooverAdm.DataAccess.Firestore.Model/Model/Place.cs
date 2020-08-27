﻿using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Firestore.Model
{
    [FirestoreData]
    public class Place : DataAccess.Model.Place
    {
        [FirestoreDocumentId]
        public DocumentReference Reference { get; set; }
        [FirestoreProperty("address")]
        public string Address { get; set; }
        [FirestoreProperty("displayname")]
        public string DisplayName { get; set; }
        [FirestoreProperty("location")]
        public GeoPoint Location { get; set; }
        [FirestoreProperty]
        public string Phone { get; set; }
        [FirestoreProperty("geohash")]
        public string Geohash { get; set; }
        [FirestoreProperty]
        public List<Timetable> Timetables { get; set; }
        [FirestoreProperty]
        public List<DocumentReference> Owners { get; set; }
        [FirestoreProperty]
        public int RatingCount { get; set; }
        [FirestoreProperty]
        public double RatingTotal { get; set; }

    }
}
