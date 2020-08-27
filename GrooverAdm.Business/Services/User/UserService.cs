using GrooverAdm.DataAccess.Dao;
using GrooverAdm.Mappers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Entities.Application.User> CreateOrUpdateUser(Entities.Application.User user)
        {
            var converted = _userMapper.ToDbEntity(user);
            var dbResult = await _userDao.CreateOrUpdateUser(converted);

            return _userMapper.ToApplicationEntity(dbResult);
        }

        public async Task<bool> DeleteUser(string id)
        {

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Entities.Application.User>> GetOwners(IEnumerable<string> references)
        {
            var dbResult = await _userDao.GetUsers(references);


            return dbResult.Select(u => _userMapper.ToApplicationEntity(u));
        }

        public async Task<Entities.Application.User> GetUser(string id)
        {
            var dbResult = await _userDao.GetUser(id);
            if(dbResult != null)
                return _userMapper.ToApplicationEntity(dbResult);
            return null;
        }

    }
}
