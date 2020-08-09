using Google.Cloud.Firestore;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Firestore
{
    public class RatingMapper : IRatingMapper<DataAccess.Firestore.Model.Rating>
    {
        private static readonly string PARENT_REF = "places";
        private static readonly string COLLECTION_REF = "ratings";
        private readonly IUserMapper<DataAccess.Firestore.Model.User> _userMapper;

        private readonly FirestoreDb _db;
        public RatingMapper(FirestoreDb db, IUserMapper<DataAccess.Firestore.Model.User> userMapper)
        {
            _db = db;
            _userMapper = userMapper;
        }

        public Entities.Application.Rating ToApplicationEntity(DataAccess.Firestore.Model.Rating dbEntity)
        {
            return new Entities.Application.Rating
            {
                Id = dbEntity.Reference.Id,
                Value = dbEntity.Value,
                User = _userMapper.ToApplicationEntity(dbEntity.User)
            };
        }

        public DataAccess.Firestore.Model.Rating ToDbEntity(Entities.Application.Rating entity)
        {
            throw new NotImplementedException();
        }

        public DataAccess.Firestore.Model.Rating ToDbEntity(Entities.Application.Rating rating, string placeId)
        {
            var reference = _db.Collection(PARENT_REF).Document(placeId).Collection(COLLECTION_REF).Document(rating.Id);
            return new DataAccess.Firestore.Model.Rating
            {
                User = _userMapper.ToDbEntity(rating.User),
                Value =  rating.Value,
                Reference = reference
            };

        }
    }
}
