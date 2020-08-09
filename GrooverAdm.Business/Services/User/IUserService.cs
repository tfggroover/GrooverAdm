using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.User
{
    public interface IUserService
    {
        public Task<Entities.Application.User> CreateUser(Entities.Application.User user);
        public Task<Entities.Application.User> GetUser(string id);
        public Task<Entities.Application.User> UpdateUser(Entities.Application.User user);
        public Task<bool> DeleteUser(string id);
    }
}
