using GrooverAdm.DataAccess.Dao;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.User
{
    public class UserService : IUserService
    {

        private readonly IUserMapper<DataAccess.Firestore.Model.User> _userMapper;
        private readonly IUserDao<DataAccess.Firestore.Model.User> _userDao;
        public UserService (IUserDao<DataAccess.Firestore.Model.User> userDao, IUserMapper<DataAccess.Firestore.Model.User> userMapper)
        {
            _userDao = userDao;
            _userMapper = userMapper;
        }

        public async Task<Entities.Application.User> CreateUser(Entities.Application.User user)
        {
            var converted = _userMapper.ToDbEntity(user);
            var dbResult = await _userDao.CreateUser(converted);

            return _userMapper.ToApplicationEntity(dbResult);
        }

        public async Task<bool> DeleteUser(string id)
        {

            throw new NotImplementedException();
        }

        public async Task<Entities.Application.User> GetUser(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Entities.Application.User> UpdateUser(Entities.Application.User user)
        {
            throw new NotImplementedException();
        }
    }
}
