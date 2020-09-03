using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Mappers.Interface
{
    public interface IUserMapper<T> : IMapper<User, T> where T : DataAccess.Model.User
    {
    }
}
