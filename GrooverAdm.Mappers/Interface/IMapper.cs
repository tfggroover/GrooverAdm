using GrooverAdm.DataAccess.Model;
using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Interface
{
    public interface IMapper<K, T> where K : IApplicationEntity where T : IDbEntity
    {

        T ToDbEntity(K entity);
        K ToApplicationEntity(T dbEntity);
    }
}
