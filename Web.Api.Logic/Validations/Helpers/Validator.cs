using System;
using Web.Api.Base.Extensions;
using Web.Api.Logic.Exceptions;

namespace Web.Api.Logic.Validations.Helpers
{
    /// <summary>
    /// Provides reusable validation helper methods
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Validates that a required field has been set
        /// </summary>
        public static void Required(string value, string displayName, int? maxLength = null)
        {
            if (value.IsNotSet())
                throw new ValidationException($"{displayName} is required");

            if (maxLength.HasValue)
                MaxLength(value, displayName, maxLength.Value);
        }

        /// <summary>
        /// Validates that a required field has been set
        /// </summary>
        public static void Required(int value, string displayName)
        {
            if (value.IsNotSet())
                throw new ValidationException($"{displayName} is required");
        }

        /// <summary>
        /// Validates that a required field has been set
        /// </summary>
        public static void Required(int? value, string displayName)
        {
            if (value.IsNotSet())
                throw new ValidationException($"{displayName} is required");
        }

        /// <summary>
        /// Validates that a required field has been set
        /// </summary>
        public static void Required(double value, string displayName)
        {
            if (value < 0)
                throw new ValidationException($"{displayName} is required");
        }

        /// <summary>
        /// Validates that a required field has been set
        /// </summary>
        public static void Required(double? value, string displayName)
        {
            if (value < 0 || value == null)
                throw new ValidationException($"{displayName} is required");
        }

        /// <summary>
        /// Validates that a required field has been set
        /// </summary>
        public static void Required(decimal value, string displayName)
        {
            if (value.IsNotSet())
                throw new ValidationException($"{displayName} is required");
        }

        /// <summary>
        /// Validates that a required field has been set
        /// </summary>
        public static void Required(DateTime value, string displayName)
        {
            if (value == DateTime.MinValue)
                throw new ValidationException($"{displayName} is required");
        }

        /// <summary>
        /// Validates that a required enum field has been set, assumes value zero is invalid
        /// </summary>
        public static void RequiredEnum<T>(T enumValue, string displayName) where T : struct, IConvertible
        {
            if (Convert.ToInt32(enumValue).IsNotSet())
                throw new ValidationException($"{displayName} is required");
        }

        /// <summary>
        /// Validates that a field doesn't exceed a given length
        /// </summary>
        public static void ExactLength(string value, string displayName, int length)
        {
            if (value.IsSet() && value.Length != length)
                throw new ValidationException($"{displayName} must be {length} {"character".SingleOrPlural(length)}");
        }

        /// <summary>
        /// Validates that a field doesn't exceed a given length
        /// </summary>
        public static void MaxLength(string value, string displayName, int maxLength)
        {
            if (value.IsSet() && value.Length > maxLength)
                throw new ValidationException($"{displayName} cannot be longer than {maxLength} {"character".SingleOrPlural(maxLength)}");
        }


        /// <summary>
        /// Validates that a field doesn't exceed a given length
        /// </summary>
        public static void MinLength(string value, string displayName, int minLength)
        {
            if (value.IsSet() && value.Length < minLength)
                throw new ValidationException($"{displayName} cannot be less than {minLength.SingleOrPluralPhrase("character")}");
        }

        /// <summary>
        /// Validates that a given condition is fulfilled
        /// </summary>
        public static void That(bool condition, string errorMessage)
        {
            if (!condition)
                throw new ValidationException(errorMessage);
        }

        /// <summary>
        /// Validates that the given object is not null, otherwise throws the given error message
        /// </summary>
        public static void NotNull(object value, string errorMessage)
        {
            That(value != null, errorMessage);
        }

        /// <summary>
        /// Validates that if an int has been set, it is a valid value for the specified enum
        /// </summary>
        public static void IsInEnum<T>(int? value, string displayName) where T : struct, IConvertible
        {
            if (value == null)
                return;

            if (value.Value.IsNotInEnum<T>())
                throw new ValidationException($"{displayName} is not a valid enum option");
        }
    }
}
