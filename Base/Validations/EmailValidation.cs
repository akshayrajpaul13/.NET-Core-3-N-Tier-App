using System;
using System.Text.RegularExpressions;
using Web.Api.Base.Extensions;

namespace Web.Api.Base.Validations
{
    /// <summary>
    /// Provides validations around email addresses
    /// </summary>
    public static class EmailValidation
    {
        /// <summary>
        /// Performs basic validation of the required fields for an email
        /// </summary>
        public static void Validate(string from, string subject, string body)
        {
            if (from.IsNotSet())
                throw new Exception("From email address is required");

            if (subject.IsNotSet())
                throw new Exception("Subject is required");

            if (body.IsNotSet())
                throw new Exception("Body is required");
        }

        /// <summary>
        /// Indicates if the given email has a valid format
        /// </summary>
        public static bool IsEmailFormatValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                               + "@"
                                + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");

            var match = regex.Match(email.Trim());
            return match.Success;
        }

        /// <summary>
        /// Throws an exception if the given email has an invalid format
        /// </summary>
        public static void ValidateEmailFormat(string email)
        {
            if (email.IsEmpty())
                throw new Exception("Email address is Required");

            if (!IsEmailFormatValid(email))
                throw new Exception($"Email address invalid: {email}");
        }

        /// <summary>
        /// Throws an exception if any of the given emails have an invalid format
        /// </summary>
        public static void ValidateEmailsFormat(params string[] emails)
        {
            foreach (var email in emails)
            {
                if (!IsEmailFormatValid(email))
                    throw new Exception($"Email address invalid: {email}");
            }
        }
    }
}
