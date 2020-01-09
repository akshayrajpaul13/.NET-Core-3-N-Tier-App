using Web.Api.Data.Enums;
using Web.Api.Data.Models;

namespace Web.Api.Logic.Validations
{
    public interface IUserValidation
    {
        void Validate(User user, string password);
        void ValidateNewPassword(User user, string password);
        void ValidateCanBeDeleted(User user);
        void ValidateRoleCanLogin(Roles role);
        void ValidateLoginTypeCanLogin(User user);
    }
}