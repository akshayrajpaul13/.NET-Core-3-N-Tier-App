using Web.Api.Data.Models;

namespace Web.Api.Logic.Services
{
    public interface ISessionService : IService
    {
        User ValidateSession(string sessionGuid, bool keepAlive, int timeoutPeriodMins);
        Session GetOrCreateSession(int userId, int loginSource, int timeoutInMins);
        void EndSession(int userId);
        void EndSession(string sessionGuid);
    }
}