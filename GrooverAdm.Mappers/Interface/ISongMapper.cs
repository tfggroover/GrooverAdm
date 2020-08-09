using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Interface
{
    public interface ISongMapper<T>: IMapper<Entities.Application.Song, T> where T : Song
    {
        DataAccess.Firestore.Model.Song ToDbEntity(Entities.Application.Song entity, string place, string playlist);
    }
}
