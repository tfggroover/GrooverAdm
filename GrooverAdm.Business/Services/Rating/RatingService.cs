using GrooverAdm.DataAccess.Dao;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.Rating
{
    public class RatingService : IRatingService
    {
        private readonly IRatingDao<DataAccess.Firestore.Model.Rating> dao;
        private readonly IRatingMapper<DataAccess.Firestore.Model.Rating> mapper;
        public RatingService(IRatingDao<DataAccess.Firestore.Model.Rating> dao, IRatingMapper<DataAccess.Firestore.Model.Rating> mapper)
        {
            this.dao = dao;
            this.mapper = mapper;
        }
        public async Task<Entities.Application.Rating> RatePlace(string placeId, double value, string user)
        {
            var converted = mapper.ToDbEntity(new Entities.Application.Rating { Id = user, Value = value });
            var dbResult = await this.dao.CreateOrUpdateRating(converted, placeId);

            return mapper.ToApplicationEntity(dbResult);
        }


        public async Task<bool> DeleteRating(string placeId, string user)
        {
            return await this.dao.DeleteRating(placeId, user);
        }
    }
}
