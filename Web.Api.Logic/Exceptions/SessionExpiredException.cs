using System;

namespace Web.Api.Logic.Exceptions
{
    public class SessionExpiredException : Exception
    {
        public SessionExpiredException() : base("Your session has expired. Please login again.")
        {
        }
    }
}
