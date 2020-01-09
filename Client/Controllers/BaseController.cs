using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Web.Api.Web.Shared.Helpers.Keys;

namespace Web.Api.Client.Controllers
{
    public class BaseController<T> : Controller
    {
        protected readonly ILogger<T> _logger;

        public BaseController(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected void SetViewError(Exception ex, bool showInnerException = true)
        {
            var errorMessage = showInnerException ? GetInnerExceptionMessage(ex) : ex.Message;
            ModelState.AddModelError("", errorMessage);
            ViewData[ViewDataKeys.ErrorMessage] = errorMessage;
        }

        private static string GetInnerExceptionMessage(Exception ex)
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
        public JsonResult ThrowJsonError(Exception ex)
        {
            var message = ex.Message;
            return ThrowJsonError(message);
        }

        /// <summary>
        /// Converts exception into a JSON format suitable for getting an error message through for display
        /// </summary>
        public JsonResult ThrowJsonError(string message)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(new { Message = message });
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

            throw new Exception(sb.ToString());//.ToHtml());
        }

        /// <summary>
        /// Provides safe redirect that will prevent redirecting to a url outside of the site
        /// </summary>
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            //returnUrl = NetworkHelper.DecodeUrlIfNeeded(returnUrl);

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}