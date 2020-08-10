using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Dao
{
    public interface IUserDao<T> where T : User
    {
        Task<T> CreateUser(T user);
        Task<T> GetUser(string id);
        Task<T> UpdateUser(T user);
        Task<bool> DeleteUser(string id);

    }
}
