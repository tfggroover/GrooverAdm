using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Dao
{
    public interface IRatingDao<T> where T : Rating
    {
        Task<T> CreateOrUpdateRating(T rating, string placeId);
        Task<bool> DeleteRating(string placeId, string ratingId);
    }
}
