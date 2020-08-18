using Google.Cloud.Firestore;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrooverAdm.Mappers.Firestore
{
    public class SongMapper : ISongMapper<DataAccess.Firestore.Model.Song>
    {
        private readonly string COLLECTION_REF = "songs";
        private readonly string PLACES_REF = "places";
        private readonly string MUSIC_REF = "placeMusic";
        private readonly string RECOGNIZED_REF = "recognizedSongs";
        private readonly FirestoreDb _db;

        public SongMapper(FirestoreDb db)
        {
            _db = db;
        }

        public Entities.Application.Song ToApplicationEntity(DataAccess.Firestore.Model.Song dbEntity)
        {
            return new Entities.Application.Song
            {
                Id = dbEntity.Reference.Id,
                Name = dbEntity.Name,
                Artists = dbEntity.Artists.Select(a => new Entities.Application.Artist { Id = a.Id, Name = a.Name }).ToList()
            };
        }

        public DataAccess.Firestore.Model.Song ToDbEntity(Entities.Application.Song entity)
        {
            var reference = _db.Collection(COLLECTION_REF).Document(entity.Id);
            return new DataAccess.Firestore.Model.Song
            {
                Reference = reference,
                Name = entity.Name,
                Artists = entity.Artists.Select(a => new DataAccess.Firestore.Model.Artist { Id = a.Id, Name = a.Name }).ToList()
            };
        }

        public DataAccess.Firestore.Model.Song ToDbEntity(Entities.Application.Song entity, string place)
        {
            var reference = _db.Collection(PLACES_REF).Document(place).Collection(RECOGNIZED_REF).Document(entity.Id);
            return new DataAccess.Firestore.Model.Song
            {
                Reference = reference,
                Name = entity.Name,
                Artists = entity.Artists.Select(a => new DataAccess.Firestore.Model.Artist { Id = a.Id, Name = a.Name }).ToList()
            };
        }

        public DataAccess.Firestore.Model.Song ToDbEntity(Entities.Application.Song entity, string place, string playlist)
        {
            var reference = _db.Collection(PLACES_REF).Document(place).Collection(MUSIC_REF).Document(playlist).Collection(COLLECTION_REF).Document(entity.Id);
            return new DataAccess.Firestore.Model.Song
            {
                Reference = reference,
                Name = entity.Name,
                Artists = entity.Artists.Select(a => new DataAccess.Firestore.Model.Artist { Id = a.Id, Name = a.Name }).ToList()
            };
        }
    }
}
