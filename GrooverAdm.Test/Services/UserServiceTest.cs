using FirebaseAdmin;
using Google.Cloud.Firestore.V1;
using GrooverAdm.Business.Services.User;
using GrooverAdm.DataAccess.Dao;
using GrooverAdm.DataAccess.Firestore.Model;
using GrooverAdm.Mappers.Interface;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace GrooverAdm.Test.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserMapper<DataAccess.Firestore.Model.User>> _userMapper;
        private Mock<IUserDao<DataAccess.Firestore.Model.User>> _userDao;
        private Mock<FirebaseApp> _firebase;

        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            _userMapper = new Mock<IUserMapper<DataAccess.Firestore.Model.User>>();
            _userDao = new Mock<IUserDao<DataAccess.Firestore.Model.User>>();
            _firebase = new Mock<FirebaseApp>();
        }

        [Test]
        public void NameAdmin_ValidUser_Ok()
        {
            //Act
            var id = "1";
            User currDbUser = new User { Admin = true};
            _userDao.Setup(a => a.GetUser(id)).Returns(Task.FromResult(currDbUser));
            _userDao.Setup(a => a.SetAdmin(id)).Returns(Task.FromResult(currDbUser));
            User dbUser = new User { Admin = true };
            GrooverAdm.Entities.Application.User result = new Entities.Application.User { Admin = true };
            _userMapper.Setup(a => a.ToApplicationEntity(dbUser)).Returns(result);

            //Arrange

            IUserService userService = new UserService(_userDao.Object, _userMapper.Object, _firebase.Object);
            var res = userService.NameAdmin(id, id);

            //Assert


        }

        [Test]
        public void NameAdmin_InvalidUser_NotOk()
        {

        }


    }
}
