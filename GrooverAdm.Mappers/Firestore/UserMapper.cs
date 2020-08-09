using Google.Cloud.Firestore;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Firestore
{
    public class UserMapper : IUserMapper<DataAccess.Firestore.Model.User>
    {
        private static readonly string COLLECTION_REF = "users";

        private readonly FirestoreDb _db;
        public UserMapper(FirestoreDb db)
        {
            _db = db;
        }
        public Entities.Application.User ToApplicationEntity(DataAccess.Firestore.Model.User dbEntity)
        {
            return new Entities.Application.User
            {
                Born = dbEntity.Born,
                CurrentToken = dbEntity.CurrentToken,
                DisplayName = dbEntity.DisplayName,
                ExpiresIn = dbEntity.ExpiresIn,
                TokenEmissionTime = dbEntity.TokenEmissionTime,
                Id = dbEntity.Reference.Id
            };
        }

        public DataAccess.Firestore.Model.User ToDbEntity(Entities.Application.User entity)
        {
            var reference = _db.Collection(COLLECTION_REF).Document(entity.Id);
            return new User
            {
                Born = entity.Born,
                CurrentToken = entity.CurrentToken,
                DisplayName = entity.DisplayName,
                ExpiresIn = entity.ExpiresIn,
                TokenEmissionTime = entity.TokenEmissionTime,
                Reference = reference
            };
        }
    }
}
