using Google.Cloud.Firestore;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Firestore.Dao
{
    public class UserFirestoreDao : IUserDao<User>
    {
        private readonly FirestoreDb _db;

        private static readonly string COLLECTION_REF = "users";
        private static readonly string PLACE_REF = "places";
        private static readonly string RECOGNIZED_REF = "recognizedSongs";

        public UserFirestoreDao(FirestoreDb db)
        {
            _db = db;
        }


        public async Task<User> CreateOrUpdateUser(User user)
        {
            var reference = user.Reference;
            var snap = await reference.GetSnapshotAsync();
            if (!snap.Exists)
                await reference.CreateAsync(user);
            else
            {
                var old = snap.ConvertTo<User>();
                old.Born = user.Born;
                old.CurrentToken = user.CurrentToken;
                old.DisplayName = user.DisplayName;
                old.Email = user.Email;
                old.ExpiresIn = user.ExpiresIn;
                old.TokenEmissionTime = user.TokenEmissionTime;
                old.RefreshToken = string.IsNullOrEmpty(user.RefreshToken) ? old.RefreshToken : user.RefreshToken;
                await reference.SetAsync(old);
            }
            snap = await reference.GetSnapshotAsync();
            return snap.ConvertTo<User>();
        }

        public async Task<bool> DeleteUser(string id)
        {
            var res = await _db.Collection(COLLECTION_REF).Document(id).DeleteAsync();
            if (res.UpdateTime != null)
                return true;

            return false;
        }

        public async Task<User> GetUser(string id)
        {
            var snap = await _db.Collection(COLLECTION_REF).Document(id).GetSnapshotAsync();
            if (snap.Exists)
                return snap.ConvertTo<User>();

            return null;
        }

        public async Task<IEnumerable<User>> GetUsers(IEnumerable<string> references)
        {
            var res = await references.ToAsyncEnumerable().SelectAwait(async r =>
            {
                var snap = await _db.Collection(COLLECTION_REF).Document(r).GetSnapshotAsync();
                if (snap.Exists)
                    return snap.ConvertTo<User>();
                return null;
            }).Where(u => u != null).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<User>> GetUsers(int page, int pageSize, string name, bool admin)
        {
            var query = _db.Collection(COLLECTION_REF).WhereEqualTo(nameof(User.Admin), admin);
            if(!string.IsNullOrWhiteSpace(name))
               query = query.WhereEqualTo(nameof(User.DisplayName), name);
            query = query.Limit(pageSize).Offset((page - 1) * pageSize);

            var res = await query.GetSnapshotAsync();

            return res.Select(d => d.ConvertTo<User>());
        }

        public async Task<User> SetAdmin(string userId)
        {
            var reference = _db.Collection(COLLECTION_REF).Document(userId);

            await _db.Collection(COLLECTION_REF).Document(userId).UpdateAsync(new Dictionary<string, object> { { nameof(User.Admin), true } });

            return (await reference.GetSnapshotAsync()).ConvertTo<User>();
        }
    }
}
