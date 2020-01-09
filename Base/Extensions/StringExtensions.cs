using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Web.Api.Base.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Indicates if a string is null, empty or has whitespace
        /// </summary>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Indicates if a string contains a value that is not null, empty or whitespace
        /// </summary>
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Removes the given clause from the left of the string, if found
        /// </summary>
        public static string RemoveLeft(this string value, string clause)
        {
            if (string.IsNullOrEmpty(clause))
            {
                throw new System.ArgumentException("Argument clause cannot be null");
            }

            if (string.IsNullOrEmpty(value))
                return value;

            var index = value.IndexOf(clause);
            if (index == -1) return value;

            if (index == 0)
                return value.Substring(clause.Length);

            return value;
        }

        /// <summary>
        /// Removes the given clause from the right of the string, if found
        /// </summary>
        public static string RemoveRight(this string value, string clause)
        {
            if (string.IsNullOrEmpty(clause))
            {
                throw new System.ArgumentException("Argument clause cannot be null");
            }

            if (string.IsNullOrEmpty(value))
                return value;

            var index = value.IndexOf(clause);
            if (index == -1) return value;

            if (value.Length - clause.Length == index)
                return value.Substring(0, index);

            return value;
        }

        /// <summary>
        /// Removes everything in a string to the left of a given clause
        /// E.g. "abcdef".LeftOf("d") will return "abc"
        /// </summary>
        public static string LeftOf(this string value, string clause)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var index = value.IndexOf(clause);
            if (index == -1)
                return value;

            return value.Substring(0, index);
        }

        /// <summary>
        /// Removes everything in a string to the right of a given clause. The last instance of the clause will be used.
        /// E.g. "abdcdef".RightOf("d") will return "ef"
        /// </summary>
        public static string RightOf(this string value, string clause)
        {
            if (string.IsNullOrEmpty(clause))
            {
                throw new ArgumentException("Argument clause cannot be null");
            }

            if (string.IsNullOrEmpty(value))
                return value;

            var index = value.LastIndexOf(clause);
            if (index == -1)
                return value;

            return value.Substring(index + clause.Length, value.Length - (index + clause.Length));
        }

        /// <summary>
        /// Returns the n-leftmost characters of the string, or the whole string if there are
        /// not enough characters
        /// </summary>
        public static string Left(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length <= length)
                return value;

            return value.Substring(0, length);
        }

        /// <summary>
        /// Returns the n-rightmost characters of the string, or the whole string if there are
        /// not enough characters
        /// </summary>
        public static string Right(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length <= length)
                return value;

            return value.Substring(value.Length - length, length);
        }

        /// <summary>
        /// Repeats the given string for the specified number of times
        /// </summary>
        public static string Repeat(this string value, int timesToRepeat)
        {
            if (value == null) return null;

            var sb = new StringBuilder();
            for (int i = 0; i < timesToRepeat; i++)
            {
                sb.Append(value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Removes all whitespace (spaces, tabs, etc) from within and around a string
        /// </summary>
        public static string RemoveWhiteSpace(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Removes all of the specified phrases from a string.
        /// This behaves the same way as mystring.Replace("x","").Replace("y","") but allows combinations, so the usage
        /// would be mystring.Remove("x", "y").
        /// Note: This method may perform poorly on large amounts of data.
        /// </summary>
        public static string RemovePhrases(this string value, params string[] phrases)
        {
            if (string.IsNullOrEmpty(value)) return value;

            if (phrases == null || !phrases.Any())
                return value;

            foreach (var phrase in phrases)
            {
                value = value.Replace(phrase, "");
            }
            return value;
        }

        public static string RemoveChar(this string input, char charToRemove)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new System.ArgumentException("Argument input cannot be null");
            }

            return new string(input.ToCharArray().Where(c => c != charToRemove).ToArray());
        }

        public static string Padding(this string value, char paddingCharacter, int totalLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new System.ArgumentException("Argument value cannot be null");
            }

            return Repeat(paddingCharacter.ToString(), totalLength - value.Length) + value;
        }

        /// <summary>
        /// Shows a value with a positive or negative symbol in front (e.g. +1 instead of just 1)
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="isZeroPositive">If true, zero is shown as +0</param>
        public static string WithPlusOrMinus(this decimal value, bool isZeroPositive)
        {
            var symbol = value >= 0 ? "+" : "-";
            if (!isZeroPositive && value == 0) symbol = "";
            return String.Format("{0}{1}", symbol, Math.Abs(value));
        }

        /// <summary>
        /// Shows a value with a positive or negative symbol in front (e.g. +1 instead of just 1)
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="isZeroPositive">If true, zero is shown as +0</param>
        public static string WithPlusOrMinus(this decimal value)
        {
            const bool isZeroPositive = false;
            var valueWithSymbol = WithPlusOrMinus(value, isZeroPositive);
            return valueWithSymbol;
        }

        /// <summary>
        /// Splits a pascal-case string into words
        /// </summary>
        public static string Depascalise(this string value)
        {
            if (value == null) return null;
            if (string.IsNullOrWhiteSpace(value)) return "";

            var depascalised = Regex.Replace(value, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            depascalised = depascalised.Replace(" and ", " & ", StringComparison.InvariantCultureIgnoreCase);

            return depascalised;
        }

        public static string ReplaceChars(this string s, string newVal)
        {
            var separators = new char[] { ' ', '.', '.', '/', ';', ',', '\r', '\t', '\n' };

            var temp = s.Split(separators);
            return string.Join(newVal, temp);
        }

        /// <summary>
        /// Converts a file size into a readable format
        /// </summary>
        public static string ToFileSize(this int filesize)
        {
            return filesize.To<long>().ToFileSize();
        }

        /// <summary>
        /// Converts a file size into a readable format
        /// </summary>
        public static string ToFileSize(this long filesize)
        {
            // From: http://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
            // Uses the same tech as Windows Explorer
            var sb = new StringBuilder(11);
            StrFormatByteSize(filesize, sb, sb.Capacity);


            return sb.ToString().Replace(",", ".");
        }

        /// <summary>
        /// Used by ToFileSize to convert a file size into a readable format
        /// </summary>
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        /// <summary>
        /// Shortens a string to the given length if required, optionally adding an ellipsis (...) which counts as one of the characters
        /// </summary>
        public static string Shorten(this string value, int maxLength, bool addEllipsisIfShortened = true)
        {
            if (string.IsNullOrEmpty(value)) return "";

            if (value.Length <= maxLength) return value;

            if (addEllipsisIfShortened)
                return value.Substring(0, maxLength - 1) + "…";

            return value.Substring(0, maxLength);
        }

        ///// <summary>
        ///// Returns either the single or plural form of the given word, depending on the count provided.
        ///// This assumes you're passing in the single form of the word.
        ///// </summary>
        public static string SingleOrPlural(this string word, int count)
        {
            if (count == 1 || count == -1)
                return word;

            // zero counts as plural
            return word.Pluralize();
        }

        /// <summary>
        /// Returns either the single or plural form of the given word, depending on the count provided.
        /// This allows you to explicitly provide the single or plural form, otherwise you should use the
        /// overload which automates that.
        /// </summary>
        public static string SingleOrPlural(this int count, string singleWord, string pluralWord)
        {
            if (count == 1 || count == -1)
                return singleWord;

            // zero counts as plural
            return pluralWord;
        }

        /// <summary>
        /// Returns either the single or plural form of the given word, depending on the count provided.
        /// Uses .Net library to work out the plural form of a word.
        /// </summary>
        public static string SingleOrPlural(this int count, string singleWord)
        {
            return count.SingleOrPlural(singleWord, singleWord.Pluralize());
        }

        /// <summary>
        /// Gets a complete phrase comprising a count value on a single or plural word with it.
        /// Uses .Net library to work out the plural form of a word.
        /// </summary>
        public static string SingleOrPluralPhrase(this int count, string singleWord)
        {
            return $"{count} {count.SingleOrPlural(singleWord, singleWord.Pluralize())}";
        }

        /// <summary>
        /// Gets a complete phrase comprising a count value on a single or plural word with it.
        /// This allows you to explicitly provide the single or plural form, otherwise you should use the
        /// overload which automates that.
        /// </summary>
        public static string SingleOrPluralPhrase(this int count, string singleWord, string pluralWord)
        {
            return $"{count} {count.SingleOrPlural(singleWord, pluralWord)}";
        }

        /// <summary>
        /// Returns the plural form of the given single-form word
        /// </summary>
        public static string Pluralize(this string word)
        {
            return word;
            // uses .Net framework service, installed in GAC
            // NOTE: if this is not working well, consider the Humanizr framework
            //return PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-GB")).Pluralize(word);
        }

        /// <summary>
        /// Returns the single form of the given plural-form word
        /// </summary>
        public static string Singularize(this string word)
        {
            return word;
            // uses .Net framework service, installed in GAC
            // NOTE: if this is not working well, consider the Humanizr framework
            //return PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-GB")).Singularize(word);
        }

        /// <summary>
        /// Combines non-null dictionary values into a single string
        /// </summary>
        public static string Join(this Dictionary<string, string> values, string joinCharacters = "&", params string[] ignoreKeys)
        {
            var result = new StringBuilder();
            foreach (var key in values.Keys)
            {
                var value = values[key];

                if (!ignoreKeys.Contains(key) && value != null)
                {
                    if (result.Length > 0)
                        result.Append(joinCharacters);
                    result.Append(key + "=" + value);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns the given string value if it's not null, otherwise returns the default value provided
        /// </summary>
        public static string GetValueOrEmpty(this string value, string emptyString = "")
        {
            if (value == null)
                return emptyString;

            return value;
        }

        /// <summary>
        /// Converts a string to a name suitable for css-style naming (e.g. SomeValue > some-value)
        /// </summary>
        public static string ToCssName(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var words = value.Depascalise().ToLower().Split(' ');
            return string.Join("-", words);
        }

        /// <summary>
        /// Indicates if the given value is one of the list of values provided
        /// </summary>
        public static bool IsIn(this string value, params string[] values)
        {
            if (values == null) return false;
            return values.Contains(value);
        }

        /// <summary>
        /// Indicates if the given value is one of the list of values provided
        /// </summary>
        public static bool IsIn(this string value, IEnumerable<string> values)
        {
            if (values == null) return false;
            return values.Contains(value);
        }

        /// <summary>
        /// Replaces all occurances of a string phrase
        /// From https://stackoverflow.com/questions/244531/is-there-an-alternative-to-string-replace-that-is-case-insensitive
        /// </summary>
        public static string Replace(this string value, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = value.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(value.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = value.IndexOf(oldValue, index, comparison);
            }
            sb.Append(value.Substring(previousIndex));

            return sb.ToString();
        }

        /// <summary>
        /// Returns a value out of 100 in the format of a percentage. Defaults to 2 decimal places but can be overridden.
        /// </summary>
        public static string ToPercentage(this decimal value, int decimalPlaces = 2)
        {
            return $"{Math.Round(value, decimalPlaces)}%";
        }

        /// <summary>
        /// Converts a comma-separated list of int values into a list of type int.
        /// </summary>
        public static List<int> ToIntList(this string value, char separator = ',')
        {
            return value.Split(separator).Select(x => x.ToInt()).ToList();
        }

        public static bool IsNumeric(this string numericString)
        {
            double retNum;

            bool isNum = Double.TryParse(numericString, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static string RemoveSpecialCharsAndSpaces(this string value)
        {
            return Regex.Replace(value, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        /// <summary>
        /// Checks if a given string contains another string phrase, ignoring case and handling potential nulls
        /// </summary>
        public static bool ContainsSafe(this string value, string searchPhrase)
        {
            return value.GetValueOrEmpty().ToLower().Contains(searchPhrase.GetValueOrEmpty().ToLower());
        }

        /// <summary>
        /// Checks if a given string matches another string phrase, ignoring case and whitespace and handling potential nulls
        /// </summary>
        public static bool EqualsSafe(this string value, string comparisonValue)
        {
            return value.GetValueOrEmpty().ToLower().Trim() == comparisonValue.GetValueOrEmpty().ToLower().Trim();
        }

        /// <summary>
        /// TODO: rework/rename
        /// </summary>
        public static string ToDate(this string value)
        {
            try
            {
                var date = Convert.ToDateTime(value);
                return date.ToDateString();
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// Capitalizes the first letter of each word in a sentence. 
        /// Eg 1. the quick brown fox => The Quick Brown Fox 
        /// Eg 2. THE QUICK BROWN FOX => The Quick Brown Fox
        /// </summary>
        public static string ToTitleCase(this string word)
        {
            if (word.IsNotSet())
                return "";

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(word.ToLower());
        }

        //TODO ES 2018-02-09: Move all usages of the HtmlHelperExtensions ToHtml to use this instead
        /// <summary>
        /// Converts a string value into suitable html form.  Useful for converting newlines into html cr's.
        /// </summary>
        public static string ToHtmlNew(this object value)
        {
            if (value == null) return "";

            return value.ToString().Replace(Environment.NewLine, "<br/>");
        }
    }
}
