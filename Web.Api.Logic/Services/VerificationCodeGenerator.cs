using System;
using System.Security.Cryptography;
using System.Text;
using Web.Api.Data.DbContexts;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Logic.Services
{
    public static class VerificationCodeGenerator
    {
        /// <summary>
        /// Generates a verification code used to confirm a user via email
        /// </summary>
        public static string GenerateVerificationCode(UnitOfWork<AppDbContext> uow, User user)
        {
            var verificationCodeLength = 5;

            return GenerateToken(verificationCodeLength, (token) => uow.UserQueries.DoesVerificationCodeExist(token));
        }

        /// <summary>
        /// Generates the code.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private static string Create(int length)
        {
            var chars = "ABCDEFGHJKMNPQRTUVWXY346789".ToCharArray();
            var data = new byte[1];

            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[length];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(length);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns true when a duplicate is found
        /// </summary>
        /// <param name="token">The token value that will be passed to this method by <see cref="GenerateToken"/></param>
        /// <returns></returns>
        private delegate bool CheckForDuplicates(string token);

        /// <summary>
        /// Generates alphanumeric tokens of the required length, such that there is no other existing token
        /// as per the supplied list.  Result is a combination of letters
        /// and upper-case characters.  Tricky letters like O, L and I are excluded.
        /// </summary>
        /// <param name="length">The length of the token</param>
        /// <param name="duplicateFound">Method that will check if the newly generated token already exists.</param>
        /// <returns></returns>
        private static string GenerateToken(int length, CheckForDuplicates duplicateFound)
        {
            string token;

            if (duplicateFound == null)
            {
                throw new Exception("Please provide a method to check for duplicates");
            }

            do
            {
                token = Create(length);

            } while (duplicateFound(token));

            return token;
        }
    }
}
