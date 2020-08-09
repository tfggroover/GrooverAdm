using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Dao
{
    public interface IUserDao<T> where T : User
    {
        public Task<T> CreateUser(T user);
        public Task<T> GetUser(string id);
        public Task<T> UpdateUser(T user);
        public Task<bool> DeleteUser(string id);

    }
}
