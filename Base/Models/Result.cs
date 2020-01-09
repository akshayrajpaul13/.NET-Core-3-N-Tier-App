using System;
using System.Collections.Generic;

namespace Web.Api.Base.Models
{
    public class Result
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public List<Result> Results { get; set; }

        public static Result Success(string message = null)
        {
            return new Result
            {
                Successful = true,
                Message = message,
            };
        }

        public static Result Failed(string message)
        {
            return new Result
            {
                Successful = false,
                Message = message,
            };
        }

        public static Result Error(Exception ex)
        {
            return Error(ex.Message, ex);
        }

        public static Result Error(string message, Exception ex)
        {
            return new Result
            {
                Successful = false,
                Message = message,
                Exception = ex,
            };
        }

        public void ThrowIfFailed()
        {
            if (Exception != null)
                throw Exception;

            if (!Successful)
                throw new Exception(Message);
        }
    }
}