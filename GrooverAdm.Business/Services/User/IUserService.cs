using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.User
{
    public interface IUserService
    {
        Task<Entities.Application.User> CreateOrUpdateUser(Entities.Application.User user);
        Task<Entities.Application.User> GetUser(string id);
        Task<bool> DeleteUser(string id);
    }
}
