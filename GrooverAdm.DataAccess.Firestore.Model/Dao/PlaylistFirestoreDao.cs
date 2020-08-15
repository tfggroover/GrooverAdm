using Google.Cloud.Firestore;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Exceptions;
using GrooverAdm.DataAccess.Firestore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Firestore.Dao
{
    public class PlaylistFirestoreDao : IPlaylistDao<Playlist>
    {

        private readonly string PLACE_REF = "places";
        private readonly string COLLECTION_REF = "placeMusic";
        private readonly string PLAYLIST_REF = "mainPlaylist";
        private readonly FirestoreDb _db;

        public PlaylistFirestoreDao(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<Playlist> CreatePlaylist(Playlist playlist)
        {
            var reference = playlist.Reference;
            if ((await reference.GetSnapshotAsync()).Exists)
                throw new AlreadyExistingEntityException("There already exists a main Playlist. Perhaps you intended to update it?");
            await reference.CreateAsync(playlist);
            return (await reference.GetSnapshotAsync()).ConvertTo<Playlist>();
        }

        public async Task<bool> DeletePlaylist(string place)
        {
            var reference = _db.Collection(PLACE_REF).Document(place).Collection(COLLECTION_REF).Document(PLAYLIST_REF);
            await reference.DeleteAsync();
            return (await reference.GetSnapshotAsync()).Exists;
        }

        public async Task<Playlist> GetPlaylist(string place)
        {
            var reference = _db.Collection(PLACE_REF).Document(place).Collection(COLLECTION_REF).Document(PLAYLIST_REF);
            var snapshot = await reference.GetSnapshotAsync();
            if (snapshot.Exists)
                return snapshot.ConvertTo<Playlist>();

            return null;
        }

        public async Task<Playlist> UpdatePlaylist(Playlist playlist)
        {
            var reference = playlist.Reference;
            if ((await reference.GetSnapshotAsync()).Exists)
                throw new AlreadyExistingEntityException("There already exists a main Playlist. Perhaps you intended to update it?");
            await reference.SetAsync(playlist);
            return (await reference.GetSnapshotAsync()).ConvertTo<Playlist>();
        }
    }
}
