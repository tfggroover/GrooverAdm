using FirebaseAdmin;
using GrooverAdm.Business.Services.User;
using GrooverAdm.Common;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.Mappers.Interface;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace GrooverAdm.Test.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserMapper<DataAccess.Firestore.Model.User>> _userMapper;
        private Mock<IUserDao<DataAccess.Firestore.Model.User>> _userDao;

        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            _userMapper = new Mock<IUserMapper<DataAccess.Firestore.Model.User>>();
            _userDao = new Mock<IUserDao<DataAccess.Firestore.Model.User>>();
        }

        [Test]
        public async Task NameAdmin_ValidUser_Ok()
        {
            //Act
            var id = "1";
            User currDbUser = new User { Admin = true};
            User dbUser = new User { Admin = true };
            GrooverAdm.Entities.Application.User result = new Entities.Application.User { Admin = true };
            _userDao.Setup(a => a.GetUser(id)).Returns(Task.FromResult(currDbUser));
            _userDao.Setup(a => a.SetAdmin(id)).Returns(Task.FromResult(currDbUser));
            _userMapper.Setup(a => a.ToApplicationEntity(dbUser)).Returns(result);

            //Arrange

            IUserService userService = new UserService(_userDao.Object, _userMapper.Object, null); //This makes so that we can't test fireAuth methods, sucks
            var res = await userService.NameAdmin(id, id);

            //Assert
            Assert.IsTrue(res.Admin);

        }

        [Test]
        public void NameAdmin_InvalidUser_NotOk()
        {
            //Act
            var id = "1";
            User currDbUser = new User { Admin = false };
            User dbUser = new User { Admin = true };
            GrooverAdm.Entities.Application.User result = new Entities.Application.User { Admin = true };
            _userDao.Setup(a => a.GetUser(id)).Returns(Task.FromResult(currDbUser));
            _userDao.Setup(a => a.SetAdmin(id)).Returns(Task.FromResult(currDbUser));
            _userMapper.Setup(a => a.ToApplicationEntity(dbUser)).Returns(result);

            //Arrange

            IUserService userService = new UserService(_userDao.Object, _userMapper.Object, null); //This makes so that we can't test fireAuth methods, sucks

            //Assert
            Assert.ThrowsAsync<GrooverAuthException>(async () => await userService.NameAdmin(id, id));
        }


    }
}
