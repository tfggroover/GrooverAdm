using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Interface
{
    public interface IPlaceMapper<T> : IMapper<Place, T> where T : DataAccess.Model.Place
    {

    }
}
