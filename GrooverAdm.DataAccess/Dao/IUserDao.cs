﻿using GrooverAdm.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.DataAccess.Dao
{
    public interface IUserDao<T> where T : User
    {
        Task<T> CreateOrUpdateUser(T user);
        Task<T> GetUser(string id);
        Task<bool> DeleteUser(string id);
    }
}
