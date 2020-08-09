using Google.Cloud.Firestore;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Firestore.Dao
{
    public class UserFirestoreDao : IUserDao<User>
    {
        private readonly FirestoreDb _db;

        private static readonly string COLLECTION_REF = "users";

        public UserFirestoreDao(FirestoreDb db)
        {
            _db = db;
        }


        public async Task<User> CreateUser(User user)
        {
            var reference = _db.Collection(COLLECTION_REF).Document(user.Reference.Id);
            if (!(await reference.GetSnapshotAsync()).Exists)
                await reference.CreateAsync(user);
            var snap = await reference.GetSnapshotAsync();
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
            var res = snap.ConvertTo<User>();

            return res;
        }

        public async Task<User> UpdateUser(User user)
        {
            var res = await _db.Collection(COLLECTION_REF).Document(user.Reference.Id).SetAsync(user);

            if (res.UpdateTime != null)
                return user;
            return null;
        }
    }
}
