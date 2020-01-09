using System.Threading.Tasks;
using Web.Api.Logic.Models.Account;

namespace Web.Api.Web.Shared.Helpers.Security
{
    public interface IAccountManager
    {
        Task<LoginResult> LogIn(ILoginModel model);
        Task LogOut();
    }
}