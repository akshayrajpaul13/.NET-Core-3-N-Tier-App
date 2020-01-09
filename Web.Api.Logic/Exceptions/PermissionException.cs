using System;

namespace Web.Api.Logic.Exceptions
{
    public class PermissionException : Exception
    {
        public int UserId { get; set; }

        public PermissionException(int userId, string message) : base(message)
        {
            UserId = userId;
        }
    }
}
