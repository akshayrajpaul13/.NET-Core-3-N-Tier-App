using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Web.Api.Base.Extensions
{
    public static class ConversionExtensions
    {
        /// <summary>
        /// Converts a string value to integer, and throws an exception if it fails.  Empty strings are treated as null.
        /// </summary>
        public static int? ToIntOrNull(this string value)
        {
            if (value.IsNotSet()) return null;
            return value.ToInt();
        }
        /// <summary>
        /// Converts a string value to integer, and throws an exception if it fails.  Empty strings use the provided default value.
        /// </summary>
        public static int ToIntOrDefault(this string value, int defaultValue)
        {
            if (value.IsNotSet()) return defaultValue;
            return value.ToInt();
        }

        /// <summary>
        /// Converts a string value to integer, and throws an exception if it fails.
        /// </summary>
        public static int ToInt(this string value)
        {
            return int.Parse(value);
        }

        /// <summary>
        /// Converts a double value to an integer, rounding up or down according to .Net defaults
        /// </summary>
        public static int ToInt(this double value)
        {
            return Convert.ToInt32(Math.Round(value, 0));
        }

        /// <summary>
        /// Converts a decimal value to an integer, rounding up or down according to .Net defaults
        /// </summary>
        public static int ToInt(this decimal value)
        {
            return Convert.ToInt32(Math.Round(value, 0));
        }

        /// <summary>
        /// Converts an enum value to its integer value.
        /// </summary>
        public static int ToInt(this Enum enumValue)
        {
            return (int)((object)enumValue);
        }

        /// <summary>
        /// Converts a double value to an integer, always rounding up
        /// </summary>
        public static int ToIntCeiling(this double value)
        {
            var ceiling = Math.Ceiling(value);
            return Convert.ToInt32(ceiling);
        }

        /// <summary>
        /// Converts a decimal value to an integer, always rounding up
        /// </summary>
        public static int ToIntCeiling(this decimal value)
        {
            return Convert.ToInt32(Math.Ceiling(value));
        }

        /// <summary>
        /// Converts a double value to an integer, always rounding down
        /// </summary>
        public static int ToIntFloor(this double value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        /// <summary>
        /// Converts a decimal value to an integer, always rounding down
        /// </summary>
        public static int ToIntFloor(this decimal value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        /// <summary>
        /// Converts a string value to decimal, and throws an exception if it fails.
        /// </summary>
        public static decimal ToDecimal(this string value)
        {
            return decimal.Parse(value);
        }

        /// <summary>
        /// Converts a string value to decimal, and throws an exception if it fails.  Empty string is treated as null.
        /// </summary>
        public static decimal? ToDecimalOrNull(this string value)
        {
            if (value.IsNotSet()) return null;
            return value.ToDecimal();
        }

        /// <summary>
        /// Indicates whether a decimal has been set to a positive value.
        /// Usually used for Id values.
        /// </summary>
        public static bool IsSet(this decimal value)
        {
            return value > 0;
        }

        /// <summary>
        /// Indicates whether a nullable decimal has been set to a positive value.
        /// Usually used for Id values.
        /// </summary>
        public static bool IsSet(this decimal? value)
        {
            return value.HasValue && value.Value.IsSet();
        }

        /// <summary>
        /// Indicates whether an int has been set to a positive value.
        /// Usually used for Id values.
        /// </summary>
        public static bool IsSet(this int value)
        {
            return value > 0;
        }

        //
        // Summary:
        //     Gets a value indicating whether the current System.Nullable`1 object has a valid
        //     value of its underlying type.
        //
        // Returns:
        //     true if the current System.Nullable`1 object has a value; false if the current
        //     System.Nullable`1 object has no value.
        public static bool IsSet(this int? value)
        {
            return value.HasValue && value.Value.IsSet();
        }

        /// <summary>
        /// Indicates whether an int has not been set to a positive value.
        /// Usually used for Id values.
        /// </summary>
        public static bool IsNotSet(this int value)
        {
            return !value.IsSet();
        }

        /// <summary>
        /// Indicates whether a nullable int has not been set to a positive value.
        /// Usually used for Id values.
        /// </summary>
        public static bool IsNotSet(this int? value)
        {
            return !value.IsSet();
        }

        public static bool IsNotSet(this bool? value)
        {
            return !value.IsSet();
        }

        public static bool IsSet(this bool? value)
        {
            return value.HasValue;
        }

        /// <summary>
        /// Indicates whether a decimal has not been set to a positive value.
        /// Usually used for Id values.
        /// </summary>
        public static bool IsNotSet(this decimal value)
        {
            return !value.IsSet();
        }

        /// <summary>
        /// Indicates whether a nullable decimal has not been set to a positive value.
        /// Usually used for Id values.
        /// </summary>
        public static bool IsNotSet(this decimal? value)
        {
            return !value.IsSet();
        }

        /// <summary>
        /// Indicates whether a string has been set with a value
        /// </summary>
        public static bool IsSet(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Indicates whether a string has not been set with a value
        /// </summary>
        public static bool IsNotSet(this string value)
        {
            return !value.IsSet();
        }

        /// <summary>
        /// Indicates whether a byte array contains any values
        /// </summary>
        public static bool IsSet(this byte[] value)
        {
            return value != null && value.Length > 0;
        }

        /// <summary>
        /// Indicates whether a byte array is null or empty
        /// </summary>
        public static bool IsNotSet(this byte[] value)
        {
            return !value.IsSet();
        }

        /// <summary>
        /// Indicates whether a List contains any values
        /// </summary>
        public static bool IsSet<T>(this List<T> value)
        {
            return value != null && value.Count > 0;
        }

        /// <summary>
        /// Indicates whether a List is null or empty
        /// </summary>
        public static bool IsNotSet<T>(this List<T> value)
        {
            return !value.IsSet();
        }

        /// <summary>
        /// Converts an object into a Json string
        /// </summary>
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Determines whether this string specified is json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is json; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJson(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = value.Trim();

            if ((!value.StartsWith("{") || !value.EndsWith("}")) &&
                (!value.StartsWith("[") || !value.EndsWith("]"))) return false;
            try
            {
                JToken.Parse(value);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a Json string into an object
        /// </summary>
        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Converts an object to another object, where normal casting is not possible,
        /// e.g. casting a parent type to a child type
        /// </summary>
        public static T ConvertTo<T>(this object value)
        {
            return value.ToJson().FromJson<T>();
        }

        /// <summary>
        /// Casts an object to another type
        /// </summary>
        public static T To<T>(this object value)
        {
            return To<T>(value, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Casts an object to another type
        /// </summary>
        public static T To<T>(this object value, CultureInfo cultureInfo)
        {
            // From: http://www.siepman.nl/blog/post/2012/03/06/Convert-to-unknown-generic-type-ChangeType-T.aspx

            var toType = typeof(T);

            if (value == null) return default(T);

            if (value is string)
            {
                if (toType == typeof(Guid))
                {
                    return To<T>(new Guid(Convert.ToString(value, cultureInfo)), cultureInfo);
                }
                if ((string)value == string.Empty && toType != typeof(string))
                {
                    return To<T>(null, cultureInfo);
                }
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return To<T>(Convert.ToString(value, cultureInfo), cultureInfo);
                }
            }

            if (toType.IsGenericType &&
                toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                toType = Nullable.GetUnderlyingType(toType); ;
            }

            bool canConvert = toType is IConvertible || (toType.IsValueType && !toType.IsEnum);
            if (canConvert)
            {
                return (T)Convert.ChangeType(value, toType, cultureInfo);
            }
            return (T)value;
        }

        /// <summary>
        /// Converts a comma-separated list of ints to an int array
        /// </summary>
        public static int[] ToIntArray(this string commaSeparatedList, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(commaSeparatedList))
                return null;

            var items = commaSeparatedList.Split(separator);
            var outputs = new List<int>();
            foreach (var item in items)
            {
                int output;
                if (!Int32.TryParse(item, out output))
                    throw new Exception("Invalid comma-separated list");

                outputs.Add(output);
            }

            return outputs.ToArray();
        }

        public static bool IsNull(this int? value)
        {
            return !value.HasValue;
        }

        public static bool IsNull(this decimal? value)
        {
            return !value.HasValue;
        }

        /// <summary>
        /// Returns the value of a nullable int, with zero being treated as null
        /// </summary>
        public static int? GetValueOrNull(this int? value)
        {
            if (value == null || value == 0) return null;
            return value;
        }

        /// <summary>
        /// Returns the value of an int, with zero being treated as null
        /// </summary>
        public static int? GetValueOrNull(this int value)
        {
            if (value == 0) return null;
            return value;
        }

        /// <summary>
        /// Returns the value of a string, with empty values being treated as null
        /// </summary>
        public static string GetValueOrNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        /// <summary>
        /// Returns the value of a string, with empty values returning the specified default value instead
        /// </summary>
        public static string GetValueOrDefault(this string value, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            return value;
        }

        /// <summary>
        /// Returns the value of a string, with empty values returning the specified default value instead
        /// </summary>
        public static string GetValueOrDefault(this object value, string defaultValue)
        {
            if (value == null) return defaultValue;
            return value.ToString();
        }

        /// <summary>
        /// Returns a boolean value as Yes or No
        /// </summary>
        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        /// <summary>
        /// Converts a dictionary into a name value collection
        /// </summary>
        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> values)
        {
            var nameValueCollection = new NameValueCollection();
            foreach (var kvp in values)
            {
                nameValueCollection.Add(kvp.Key, kvp.Value);
            }
            return nameValueCollection;
        }

        /// <summary>
        /// Indicates if the given string can be converted to a valid boolean
        /// </summary>
        public static bool IsBoolean(this string value)
        {
            if (value == "0" || value == "1")
                return true;

            bool valueOut;
            return bool.TryParse(value, out valueOut);
        }

        /// <summary>
        /// Converts the given string value to a boolean, or throws an exception if the conversion is not valid.
        /// </summary>
        public static bool ToBoolean(this string value)
        {
            if (value == "0") return false;
            if (value == "1") return true;
            if (value.ContainsSafe("yes")) return true;
            if (value.ContainsSafe("no")) return false;

            if (!value.IsBoolean())
                throw new Exception(value + " is not a valid boolean. Rather use True or False.");

            return Convert.ToBoolean(value);
        }

        /// <summary>
        /// Indicates if a string value contains a valid int
        /// </summary>
        public static bool IsInt(this string value)
        {
            int valueOut;
            return int.TryParse(value, out valueOut);
        }

        /// <summary>
        /// Indicates if a string value contains a valid decimal
        /// </summary>
        public static bool IsDecimal(this string value)
        {
            decimal valueOut;
            return decimal.TryParse(value, out valueOut);
        }

        /// <summary>
        /// Converts a byte array to a string, using UTF8 encoding
        /// </summary>
        public static string ToStringUtf8(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        /// <summary>
        /// Converts a byte array to a string, using UTF16 encoding
        /// </summary>
        public static string ToStringUnicode(this byte[] value)
        {
            return Encoding.Unicode.GetString(value);
        }


        /// <summary>
        /// Returns a list containing the given item
        /// </summary>
        public static List<T> IntoList<T>(this T value)
        {
            return new List<T> { value };
        }

        // TODO AR 2018-03-12: move somewhere else, like a MathHelper
        /// <summary>
        /// Indicates whether a given value is between two other values, inclusively
        /// </summary>
        public static bool Between(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
    }
}
