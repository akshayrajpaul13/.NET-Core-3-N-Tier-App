using Web.Api.Data.Models;
using Web.Api.Logic.Models.Account;

namespace Web.Api.Logic.Services
{
    public interface IUserService : IService
    {
        User Create(User user, string password);
        LoginResult Login(User user, string password, int loginSource, bool createSession = true);
        void Logout(string sessionGuid);
    }
}