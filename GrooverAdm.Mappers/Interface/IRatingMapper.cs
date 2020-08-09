using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Interface
{
    public interface IRatingMapper<T> : IMapper<Entities.Application.Rating, T> where T: IDbEntity
    {
        public DataAccess.Firestore.Model.Rating ToDbEntity(Entities.Application.Rating rating, string placeId);
    }
}
