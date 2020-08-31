using FirebaseAdmin;
using FirebaseAdmin.Auth;
using GrooverAdm.Common;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.Entities.Application;
using GrooverAdm.Entities.Spotify;
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
        private readonly FirebaseApp firebase;
        public UserService (IUserDao<DataAccess.Firestore.Model.User> userDao, IUserMapper<DataAccess.Firestore.Model.User> userMapper, FirebaseApp firebase)
        {
            _userDao = userDao;
            _userMapper = userMapper;
            this.firebase = firebase;
        }

        public async Task<Entities.Application.User> CreateOrUpdateUser(Entities.Application.User user)
        {
            var converted = _userMapper.ToDbEntity(user);
            var dbResult = await _userDao.CreateOrUpdateUser(converted);

            return _userMapper.ToApplicationEntity(dbResult);
        }

        public async Task<bool> DeleteUser(string id)
        {
            var auth = FirebaseAuth.GetAuth(this.firebase);

            await _userDao.DeleteUser(id);
            
            try
            {
                await auth.GetUserAsync(id); // If the user does not exist an exception is thrown
                await auth.DeleteUserAsync(id);
            }
            catch (FirebaseAuthException)
            {
                throw new GrooverException("The user specified does not exists", 400);
            }

            return true;
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

        public async Task<IEnumerable<ListableUser>> GetUsers(int page, int pageSize, string name, bool admin)
        {

            var dbResult = await _userDao.GetUsers(page, pageSize, name, admin);

            return dbResult.Select(u => new ListableUser(_userMapper.ToApplicationEntity(u)));
        }

        public async Task<Entities.Application.User> NameAdmin(string userId, string user)
        {
            var current = await _userDao.GetUser(user);
            if (!current.Admin)
                throw new GrooverAuthException("Only an admin can name new admins");
            var dbResult = await _userDao.SetAdmin(userId);

            return _userMapper.ToApplicationEntity(dbResult);
        }

        public async Task<string> GenerateToken(UserInfo userData, IAuthResponse credentials)
        {

            var auth = FirebaseAuth.GetAuth(this.firebase);

            try
            {
                await auth.GetUserAsync(userData.Id); // If the user does not exist an exception is thrown
                await auth.UpdateUserAsync(new UserRecordArgs()
                {
                    Email = userData.Email,
                    DisplayName = userData.Display_name,
                    Uid = userData.Id,
                    PhotoUrl = userData.Images.FirstOrDefault()?.Url
                });
            }
            catch (FirebaseAuthException)
            {
                await auth.CreateUserAsync(new UserRecordArgs()
                {
                    Email = userData.Email,
                    DisplayName = userData.Display_name,
                    Uid = userData.Id,
                    PhotoUrl = userData.Images.FirstOrDefault()?.Url
                });
            }

            var user = new Entities.Application.User(userData, credentials.Access_token, credentials.Expires_in, DateTime.UtcNow);
            await this.CreateOrUpdateUser(user);

            var token = await auth.CreateCustomTokenAsync(user.Id);
            return token;
        }
    }
}
