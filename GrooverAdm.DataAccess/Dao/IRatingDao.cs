using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.Dao
{
    public interface IRatingDao<T> where T : Rating
    {
        public T CreateRating(T rating, string placeId);
        public bool DeleteRating(string placeId, string ratingId);
    }
}
