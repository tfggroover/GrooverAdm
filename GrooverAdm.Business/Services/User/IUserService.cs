using GrooverAdm.Entities.Spotify;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.User
{
    public interface IUserService
    {
        Task<Entities.Application.User> CreateOrUpdateUser(Entities.Application.User user);
        Task<Entities.Application.User> GetUser(string id);
        Task<bool> DeleteUser(string id);
        Task<IEnumerable<Entities.Application.User>> GetOwners(IEnumerable<string> references);
        Task<Entities.Application.User> NameAdmin(string userId, string user);
        Task<IEnumerable<Entities.Application.ListableUser>> GetUsers(int page, int pageSize, string name, bool admin);
        Task<string> GenerateToken(UserInfo userData, IAuthResponse credentials);
    }
}
