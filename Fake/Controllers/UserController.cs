using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrooverAdm.Business.Services.User;
using GrooverAdm.Entities.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrooverAdm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Returns the currentUser
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("currentUser")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var user = GetUserId();
            return Ok(await this.userService.GetUser(user));
        }

        /// <summary>
        /// Sets a user as admin
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// 
        [HttpPatch]
        [Route("{userId}/admin")]
        public async Task<ActionResult> SetAdmin(string userId)
        {
            var user = GetUserId();

            return Ok(await this.userService.NameAdmin(userId, user));
        }

        /// <summary>
        /// Returns users according to the filters specified
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="owner">Only owners</param>
        /// <param name="admin">Only admins</param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(int page, int pageSize, string name, string email, bool owner, bool admin)
        {
            var user = GetUserId();

            return Ok(await this.userService.GetUsers(page, pageSize, name, email, owner, admin));
        }

        /// <summary>
        /// Delete the current user's account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteAccount()
        {
            var user = GetUserId();

            if (await this.userService.DeleteUser(user))
                return Ok();
            return BadRequest();
        }
    }
}