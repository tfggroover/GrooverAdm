using Google.Cloud.Firestore;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Firestore
{
    public class PlaylistMapper : IPlaylistMapper<DataAccess.Firestore.Model.Playlist>
    {

        private static readonly string COLLECTION_REF = "placeMusic";
        private static readonly string PARENT_REF = "places";
        private static readonly string MAIN_REF = "mainPlaylist";
        private static readonly string KEYWORD = "Playlist";

        private readonly FirestoreDb _db;

        public PlaylistMapper(FirestoreDb db)
        {
            _db = db;
        }

        public Entities.Application.Playlist ToApplicationEntity(DataAccess.Firestore.Model.Playlist dbEntity)
        {

            return new Entities.Application.Playlist
            {
                SnapshotVersion = dbEntity.SnapshotId,
                Id = dbEntity.Id,
                Url = dbEntity.Url,
                ImageUrl = dbEntity.ImageUrl,
                Tags = dbEntity.OcurrenceDictionary
            };
        }

        public DataAccess.Firestore.Model.Playlist ToDbEntity(Entities.Application.Playlist entity)
        {
            throw new NotImplementedException("Refer to the other overload");
        }

        public DataAccess.Firestore.Model.Playlist ToDbEntity(Entities.Application.Playlist entity, string placeId)
        {
            DocumentReference reference = _db.Collection(PARENT_REF).Document(placeId).Collection(COLLECTION_REF).Document(MAIN_REF);

            return new DataAccess.Firestore.Model.Playlist
            {
                Reference = reference,
                Id = entity.Id,
                SnapshotId = entity.SnapshotVersion,
                Url = entity.Url,
                ImageUrl = entity.ImageUrl,
                OcurrenceDictionary = entity.Tags
            };
        }
    }
}
