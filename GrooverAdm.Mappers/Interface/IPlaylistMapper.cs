using GrooverAdm.DataAccess.Model;
using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Interface
{
    public interface IPlaylistMapper<T> : IMapper<Entities.Application.Playlist, T> where T :IDbEntity
    {
        DataAccess.Firestore.Model.Playlist ToDbEntity(Entities.Application.Playlist entity, string placeId);

    }
}
