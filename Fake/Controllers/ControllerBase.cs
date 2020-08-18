using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fake.Controllers
{
    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// Get authenticated user id.        
        /// </summary>
        /// <returns></returns>
        protected string GetUserId()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            return userId;
        }
    }
}
