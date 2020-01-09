using System;
using System.Transactions;
using Web.Api.Base.Extensions;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;
using Web.Api.Logic.Exceptions;

namespace Web.Api.Logic.Services
{
    public sealed class SessionService : BaseService, ISessionService
    {
        public const int SessionLengthMinutes = 20;

        public SessionService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// Gets or Creates a User Session
        /// </summary>
        public Session GetOrCreateSession(int userId, int loginSource, int timeoutInMins = SessionLengthMinutes)
        {
            using (var transaction = new TransactionScope())
            {
                var dateTimeUtcNow = DateTime.UtcNow;

                // get user Session
                var session = Uow.SessionQueries.GetActiveUserSession(userId) ?? Uow.Sessions.Create(new Session
                {
                    UserId = userId,
                    SessionGuid = Guid.NewGuid().ToString(),
                    Start = dateTimeUtcNow,
                    End = dateTimeUtcNow.AddMinutes(timeoutInMins),
                    TimeoutInMins = timeoutInMins,
                    LoginSourceId = loginSource,
                });

                // update existing session with new End DateTime
                session.End = dateTimeUtcNow.AddMinutes(timeoutInMins);

                session = Uow.Sessions.Update(session);

                transaction.Complete();

                return session;
            }
        }

        /// <summary>
        /// Ends a session for a given user
        /// </summary>
        public void EndSession(int userId)
        {
            var session = Uow.SessionQueries.GetActiveUserSession(userId);
            if (session == null)
                return;

            EndSession(session);
        }

        /// <summary>
        /// Ends a session using the session Guid
        /// </summary>
        public void EndSession(string sessionGuid)
        {
            var session = Uow.SessionQueries.Get(sessionGuid);
            if (session == null)
                return;

            EndSession(session);
        }

        private void EndSession(Session session)
        {
            // set end time to now to prevent further use
            session.End = DateTime.UtcNow;
            Uow.Sessions.Update(session);
        }

        /// <summary>
        /// Validates the User Session is active
        /// </summary>
        public User ValidateSession(string sessionGuid, bool keepAlive = true, int timeoutPeriodMins = SessionLengthMinutes)
        {
            if (sessionGuid.IsNotSet())
                throw new SessionExpiredException();

            // Validate session exists
            var session = Uow.SessionQueries.Get(sessionGuid, true);

            // Validate user exists for the session
            // Technically user will always exist because of the non-null foreign key, but we load it anyway for the return
            var user = Uow.SessionQueries.GetUserForSession(sessionGuid);
            if (user == null)
                throw new Exception("User not found for specified session guid");

            // validate session span
            if (session.Start >= DateTime.UtcNow || session.End <= DateTime.UtcNow)
                throw new SessionExpiredException();

            // update existing session with new End DateTime
            if (keepAlive)
                ExtendSession(timeoutPeriodMins, session);

            return user;
        }

        private void ExtendSession(int timeoutPeriodMins, Session session)
        {
            // extend the session with the timeout period
            session.End = DateTime.UtcNow.AddMinutes(timeoutPeriodMins);

            Uow.Sessions.Update(session);
        }
    }
}
