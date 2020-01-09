using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Web.Api.Base.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts an enum to a list
        /// </summary>
        public static List<T> ToList<T>(bool sortAlphabetically = false) where T : struct, IConvertible
        {
            if (sortAlphabetically)
                return Enum.GetValues(typeof(T)).Cast<T>().OrderBy(x => x.ToString()).ToList();
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// Indicates if the given enum int id belongs to the specified enum
        /// </summary>
        public static bool IsInEnum<T>(this int value) where T : struct, IConvertible
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// Indicates if the given string name belongs to the specified enum, case must match
        /// </summary>
        public static bool IsInEnum<T>(this string name) where T : struct, IConvertible
        {
            var names = Enum.GetNames(typeof(T));
            return names.Any(x => x == name);
        }

        /// <summary>
        /// Indicates if the given enum int id does not belong to the specified enum
        /// </summary>
        public static bool IsNotInEnum<T>(this int value) where T : struct, IConvertible
        {
            return !value.IsInEnum<T>();
        }

        /// <summary>
        /// Indicates if the given string name does not belong to the specified enum
        /// </summary>
        public static bool IsNotInEnum<T>(this string value) where T : struct, IConvertible
        {
            return !value.IsInEnum<T>();
        }

        /// <summary>
        /// Converts an integer to the given enum
        /// </summary>
        public static T ToEnum<T>(this int value) where T : struct, IConvertible
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentException(string.Format("Value '{0}' is not valid for enum {1}", value, typeof(T).Name));
            }
            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// Converts a string name to the given enum
        /// </summary>
        public static T ToEnum<T>(this string name) where T : struct, IConvertible
        {
            if (!name.IsInEnum<T>())
            {
                throw new ArgumentException($"Name '{name}' is not valid for enum {typeof(T).Name}");
            }

            return (T)Enum.Parse(typeof(T), name);
        }

        /// <summary>
        /// Converts a nullable integer to the given enum, or returns a default if null
        /// </summary>
        public static T ToEnumOrDefault<T>(this int? value, T defaultValue) where T : struct, IConvertible
        {
            if (!value.HasValue) return defaultValue;

            return ToEnum<T>(value.Value);
        }

        /// <summary>
        /// Converts a string name to the given enum
        /// </summary>
        public static T ToEnumOrDefault<T>(this string name, T defaultValue) where T : struct, IConvertible
        {
            if (string.IsNullOrWhiteSpace(name)) return defaultValue;

            return ToEnum<T>(name);
        }

        /// <summary>
        /// Gets enum values as a strongly typed collection
        /// </summary>
        public static List<T> GetValues<T>() where T : struct, IConvertible
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// Converts the given enum to a readable string for display
        /// </summary>
        public static string ToDisplayString<T>(this T? value) where T : struct, IConvertible
        {
            if (value == null) return "";

            return value.Value.ToDisplayString();
        }

        /// <summary>
        /// Converts the given enum to a readable string for display
        /// </summary>
        public static string ToDisplayString<T>(this T value) where T : struct, IConvertible
        {
            return value.ToString().Depascalise();
        }

        /// <summary>
        /// Converts the given integer to an enum and returns the enum value as a readable string for display
        /// </summary>
        public static string ToEnumDisplayString<T>(this int value) where T : struct, IConvertible
        {
            return value.ToEnum<T>().ToDisplayString();
        }

        /// <summary>
        /// Converts the given integer to an enum and returns the enum value as a readable string for display
        /// </summary>
        public static string ToEnumDisplayString<T>(this int? value) where T : struct, IConvertible
        {
            if (value == null) return "";

            return value.Value.ToEnum<T>().ToDisplayString();
        }

        /// <summary>
        /// Indicates if the given int value is equal to the specified enum value
        /// </summary>
        public static bool EqualsEnum<T>(this int value, T enumValue) where T : struct, IConvertible
        {
            if (value.IsNotInEnum<T>())
                return false;

            var convertedValue = value.ToEnum<T>();
            return convertedValue.ToString() == enumValue.ToString();
        }

        /// <summary>
        /// Indicates if the given int value is equal to the specified enum value
        /// </summary>
        public static bool EqualsEnum<T>(this int? value, T enumValue) where T : struct, IConvertible
        {
            if (value == null) return false;

            return value.Value.EqualsEnum(enumValue);
        }

        /// <summary>
        /// Indicates if the given int value is not equal to the specified enum value
        /// </summary>
        public static bool NotEqualsEnum<T>(this int value, T enumValue) where T : struct, IConvertible
        {
            if (value.IsNotInEnum<T>())
                return true;

            var convertedValue = value.ToEnum<T>();
            return convertedValue.ToString() != enumValue.ToString();
        }

        /// <summary>
        /// Indicates if the given int value is not equal to the specified enum value
        /// </summary>
        public static bool NotEqualsEnum<T>(this int? value, T enumValue) where T : struct, IConvertible
        {
            if (value == null) return true;

            return value.Value.NotEqualsEnum(enumValue);
        }

        /// <summary>
        /// Indicates if the given int value is one of the set of enum values
        /// </summary>
        public static bool IsOneOf<T>(this int value, params T[] enumValues) where T : struct, IConvertible
        {
            foreach (var enumValue in enumValues)
            {
                if (value.EqualsEnum(enumValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates if the given int value is one of the set of enum values
        /// </summary>
        public static bool IsOneOf<T>(this int? value, params T[] enumValues) where T : struct, IConvertible
        {
            if (value == null || enumValues == null || enumValues.Length == 0) return false;

            return IsOneOf(value.Value, enumValues);
        }

        /// <summary>
        /// Indicates if the given int value is one of the set of enum values
        /// </summary>
        public static bool IsOneOf<T>(this T value, params T[] enumValues) where T : struct, IConvertible
        {
            foreach (var enumValue in enumValues)
            {
                if (value.Equals(enumValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates if the given int value is one of the set of enum values
        /// </summary>
        public static bool IsOneOf<T>(this T value, List<T> enumValues) where T : struct, IConvertible
        {
            if (enumValues.IsNotSet())
                return false;
            foreach (var enumValue in enumValues)
            {
                if (value.Equals(enumValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Converts the enum name into a suitable css-style format (e.g. SomeValue > some-value)
        /// </summary>
        public static string ToCssName<T>(this T value) where T : struct, IConvertible
        {
            return value.ToString(CultureInfo.InvariantCulture).ToCssName();
        }
    }
}
