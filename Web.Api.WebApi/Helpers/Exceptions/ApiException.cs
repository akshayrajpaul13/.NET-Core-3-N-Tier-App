using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Api.WebApi.Helpers.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {

        }
    }
}
