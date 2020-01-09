using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Api.WebApi.Helpers.Exceptions;

namespace Web.Api.WebApi.Controllers
{
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        private readonly ILogger<T> _logger;

        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected static string GetInnerExceptionMessage(Exception ex)
        {
            // Keep looping inwards until you find the innermost error
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex.Message;
        }

        protected void LogException(Exception ex, bool logValidationExceptions = false)
        {
            if (ex is ValidationException && !logValidationExceptions)
                return;

            _logger.LogError(ex, ex.Message);
        }

        protected void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        /// <summary>
        /// Converts exception into a JSON format suitable for getting an error message through for display
        /// </summary>
        protected JsonResult ThrowJsonError(Exception ex)
        {
            var message = ex.Message;
            return ThrowJsonError(message);
        }

        /// <summary>
        /// Converts exception into a JSON format suitable for getting an error message through for display
        /// </summary>
        protected JsonResult ThrowJsonError(string message)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return new JsonResult(new { Message = message });
        }

        /// <summary>
        /// Converts the model state error list into an exception containing an html-formatted message
        /// </summary>
        protected void ThrowModelStateErrors()
        {
            var sb = new StringBuilder();

            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    sb.AppendLine(error.ErrorMessage);
                }
            }

            throw new ApiException(sb.ToString());//.ToHtml());
        }
    }
}
