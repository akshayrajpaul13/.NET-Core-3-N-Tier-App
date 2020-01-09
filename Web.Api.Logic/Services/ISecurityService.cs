using Web.Api.Base.Models;
using Web.Api.Data.Models;

namespace Web.Api.Logic.Services
{
    public interface ISecurityService
    {
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
        Result IsPasswordStrong(int? tenantId, string password);
        string CreateVerificationCode(User user);
    }
}