using Google.Cloud.Firestore;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooverAdm.Common.Util;

namespace GrooverAdm.DataAccess.Firestore.Dao
{
    public class SongFirestoreDao : ISongDao<Song>
    {
        private readonly FirestoreDb _db;
        private readonly string PLACES_REF = "places";
        private readonly string MUSIC_REF = "placeMusic";
        private readonly string COLLECTION_REF = "songs";
        
        public SongFirestoreDao(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<Song> AddSong(Song song)
        {
            var reference = song.Reference;
            if (!(await reference.GetSnapshotAsync()).Exists)
                await reference.CreateAsync(song);
            var snap = await reference.GetSnapshotAsync();
            return snap.ConvertTo<Song>();
        }

        public async Task<List<Task<Song>>> AddSongs(List<Song> song)
        {
            var batch = _db.StartBatch();
            song.ForEach(s =>
            {
                batch.Create(s.Reference, s);
            });
            var results = await batch.CommitAsync();

            var res = song.Select(async s => (await s.Reference.GetSnapshotAsync()).ConvertTo<Song>()).ToList();
            return res;
        }

        public async Task<bool> DeleteSong(string place, string playlist, string song)
        {
            var reference = _db.Collection(PLACES_REF).Document(place).Collection(MUSIC_REF).Document(playlist).Collection(COLLECTION_REF).Document(song);
            await reference.DeleteAsync();
            return (await reference.GetSnapshotAsync()).Exists;
        }

        public async Task<List<Song>> GetSongs(string place, string playlist, int page, int pagesize)
        {
            
            var reference = await _db.Collection(PLACES_REF).Document(place).Collection(MUSIC_REF).Document(playlist).Collection(COLLECTION_REF).Limit(pagesize).Offset((page - 1) * pagesize).GetSnapshotAsync();

            return reference.Select(d => d.ConvertTo<Song>()).ToList();
            
        }

        public async Task<List<Task<Song>>> OverrideSongs(List<Song> song, string place, string playlist)
        {
            var reference = _db.Collection(PLACES_REF).Document(place).Collection(MUSIC_REF).Document(playlist).Collection(COLLECTION_REF);
            await reference.DeleteCollection(100);

            var batch = _db.StartBatch();
            song.ForEach(s =>
            {
                batch.Create(s.Reference, s);
            });
            var results = await batch.CommitAsync();

            var res = song.Select(async s => (await s.Reference.GetSnapshotAsync()).ConvertTo<Song>()).ToList();
            return res;
        }
    }
}
