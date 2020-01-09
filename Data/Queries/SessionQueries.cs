using System;
using System.Collections.Generic;
using System.Linq;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Data.Queries
{
    public sealed class SessionQueries : BaseQueries
    {
        public SessionQueries(IUnitOfWork repo) : base(repo)
        {
        }

        public Session Get(string sessionGuid, bool throwExceptionIfNotFound = false)
        {
            var session = Uow.Sessions.FirstOrDefault(x => x.SessionGuid == sessionGuid);
            if (throwExceptionIfNotFound && session == null)
                throw new Exception("Session not found for guid: " + sessionGuid);

            return session;
        }

        public Session GetLatest(int userId)
        {
            return Uow.Sessions.FirstOrDefault(x => x.UserId == userId,
                    sessions => sessions.OrderByDescending(x => x.End));
        }

        public List<Session> GetForUser(int userId)
        {
            return Uow.Sessions.GetAll(
                    x => x.UserId == userId,
                    sessions => sessions.OrderByDescending(x => x.End))
                .ToList();
        }

        public User GetUserForSession(string sessionGuid)
        {
            var session = Get(sessionGuid);
            if (session == null)
                return null;

            return Uow.Users.FirstOrDefault(u => u.Id == session.UserId);
        }

        /// <summary>
        /// Finds the current active Session for a specific UserId
        /// </summary>
        public Session GetActiveUserSession(int userId)
        {
            var dateUtcNow = DateTime.UtcNow;

            return Uow.Sessions.FirstOrDefault(
                x => x.UserId == userId && x.Start <= dateUtcNow && x.End >= dateUtcNow,
                x => x.OrderByDescending(x => x.Start)
            );
        }
    }
}
