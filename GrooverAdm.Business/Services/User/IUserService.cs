using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.User
{
    public interface IUserService
    {
        Task<Entities.Application.User> CreateUser(Entities.Application.User user);
        Task<Entities.Application.User> GetUser(string id);
        Task<Entities.Application.User> UpdateUser(Entities.Application.User user);
        Task<bool> DeleteUser(string id);
    }
}
