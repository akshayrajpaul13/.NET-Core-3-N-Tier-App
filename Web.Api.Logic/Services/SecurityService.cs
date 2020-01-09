using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using Web.Api.Base.Models;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;

namespace Web.Api.Logic.Services
{
    public sealed class SecurityService : BaseService, ISecurityService
    {
        public SecurityService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// Version 3:
        /// PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey, 10000 iterations.
        /// Format: { 0x01, prf (UInt32), iter count (UInt32), salt length (UInt32), salt, subkey }
        /// (All UInt32s are stored big-endian.)
        /// </summary>
        /// <param name="password">string</param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "";

            var prf = KeyDerivationPrf.HMACSHA256;
            var rng = RandomNumberGenerator.Create();
            const int iterCount = 10000;
            const int saltSize = 128 / 8;
            const int numBytesRequested = 256 / 8;

            // Produce a version 3 (see comment above) text hash.
            var salt = new byte[saltSize];
            rng.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, iterCount);
            WriteNetworkByteOrder(outputBytes, 9, saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// Verifies user provided password matches the hashed Password
        /// </summary>
        /// <param name="hashedPassword">string</param>
        /// <param name="providedPassword">string</param>
        /// <returns></returns>
        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                throw new ArgumentNullException(nameof(hashedPassword));

            if (string.IsNullOrEmpty(providedPassword))
                throw new ArgumentNullException(nameof(providedPassword));

            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // Wrong version
            if (decodedHashedPassword[0] != 0x01)
                return false;

            // Read header information
            var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
            var iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
            var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8)
            {
                return false;
            }
            var salt = new byte[saltLength];
            Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }
            var expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);
            return actualSubkey.SequenceEqual(expectedSubkey);
        }

        /// <summary>
        /// Used to create Password Hash
        /// </summary>
        /// <param name="buffer">byte array</param>
        /// <param name="offset">int</param>
        /// <param name="value">uint</param>
        private void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value);
        }

        /// <summary>
        /// Used to create Password Hash
        /// </summary>
        /// <param name="buffer">byte array</param>
        /// <param name="offset">int</param>
        /// <returns>uint</returns>
        private uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }


        /// <summary>
        /// Creates a callBackUrl link text the user will click for Password Reset and Confirm Email
        /// </summary>
        /// <param name="tenantId">Tenant object cannot be null</param>
        /// <param name="source">string must contain the following keywords {tenantId},{email},{code} </param>
        /// <param name="userEmail">string detailing user email</param>
        /// <param name="verificationCode">string</param>
        /// <returns>string</returns>
        public string CreateCallBackUrl(int tenantId, string source, string userEmail, string verificationCode)
        {
            return source.Replace("{email}", HttpUtility.UrlEncode(userEmail))
                         .Replace("{code}", HttpUtility.UrlEncode(verificationCode));
        }

        /// <summary>
        /// Checks password strength against server rules
        /// </summary>
        public Result IsPasswordStrong(int? tenantId, string password)
        {
            // You can set these from your custom service methods
            var minLen = 5;
            var maxLen = 10;
            var minDigit = 1;
            var minSpChar = 1;

            // Check for password length
            if (password == null || password.Length < minLen || password.Length > maxLen)
                return Result.Failed($"Password must be greater than {minLen} and less than {maxLen} characters long");

            // Check for Digits and Special Characters
            var digitCount = 0;
            var splCharCount = 0;
            foreach (var c in password)
            {
                if (char.IsDigit(c)) digitCount++;
                if (Regex.IsMatch(c.ToString(), @"[!#$%&'()*+,-.:;<=>?@[\\\]{}^_`|~]")) splCharCount++;
            }

            if (digitCount < minDigit)
                return Result.Failed($"Password must have at least {minDigit} digit(s).");

            if (splCharCount < minSpChar)
                return Result.Failed($"Password must have at least {minSpChar} special character(s).");

            return Result.Success();
        }

        public string CreateVerificationCode(User user)
        {
            return VerificationCodeGenerator.GenerateVerificationCode(Uow, user);
        }
    }
}
