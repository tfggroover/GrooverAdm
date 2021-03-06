﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Place : IApplicationEntity
    {
        public Place()
        {
            Owners = new List<ListableUser>();
            RecognizedMusic = new List<RecognizedSong>();
            Timetables = new List<Timetable>();
        }

        public string Id { get; set; }
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public Geolocation Location { get; set; }
        public Playlist MainPlaylist { get; set; }
        public Dictionary<string, Playlist> WeeklyPlaylists { get; set; }
        public double Ratings { get; set; }
        public int RatingCount { get; set; }
        public List<ListableUser> Owners{ get; set; }
        public string Phone { get; set; }
        public string Geohash { get; set; }
        public List<RecognizedSong> RecognizedMusic{ get; set; }
        public List<Timetable> Timetables { get; set; }
        public bool Approved { get; set; }
        public bool PendingReview { get; set; }
        public string ReviewComment { get; set; }

    }

    public class ComparedPlace : Place
    {
        public ComparedPlace() { }
        public ComparedPlace (Place p)
        {
            Id = p.Id;
            Address = p.Address;
            DisplayName = p.DisplayName;
            Location = p.Location;
            MainPlaylist = p.MainPlaylist;
            WeeklyPlaylists = p.WeeklyPlaylists;
            Ratings = p.Ratings;
            Owners = p.Owners;
            Phone = p.Phone;
            Geohash = p.Geohash;
            RecognizedMusic = p.RecognizedMusic;
            Timetables = p.Timetables;
        }
        public double Similitude { get; set; }
    }

}
