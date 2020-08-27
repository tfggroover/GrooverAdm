using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.Rating
{
    public interface IRatingService
    {
        Task<Entities.Application.Rating> RatePlace(string placeId, double value, string user);
        Task<bool> DeleteRating(string placeId, string user);
    }
}
