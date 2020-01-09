using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Web.Api.Base.Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// Returns the earliest of a list of dates, or null if they are all null.  There is no limit
        /// on the number of parameters allowed.
        /// </summary>
        public static DateTime? EarliestOf(params DateTime?[] dates)
        {
            if (dates == null || dates.Length == 0)
                return null;

            var nonNullDates = dates.Where(x => x != null).ToList();
            if (nonNullDates.NullOrEmpty())
                return null;

            return nonNullDates.Min();
        }

        /// <summary>
        /// Returns the latest of a list of dates, or null if they are all null.  There is no limit
        /// on the number of parameters allowed.
        /// </summary>
        public static DateTime? LatestOf(params DateTime?[] dates)
        {
            if (dates == null || dates.Length == 0)
                return null;

            var nonNullDates = dates.Where(x => x != null).ToList();
            if (nonNullDates.NullOrEmpty())
                return null;

            return nonNullDates.Max();
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date string format
        /// </summary>
        public static string ToDateString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year/month/date string format for Compuscan
        /// </summary>
        public static string ToCompuscanDateString(this DateTime value)
        {
            return value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year/month/date string format for Compuscan
        /// </summary>
        public static string ToCompuscanDateString(this DateTime? value)
        {
            if (value == null) return "";

            return value.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable day month string format eg. 9 Aug
        /// </summary>
        public static string ToShortDateMonthString(this DateTime value)
        {
            return value.ToString("d MMM", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable day month string format eg. 9 Aug
        /// </summary>
        public static string ToShortDateMonthString(this DateTime? value)
        {
            if (value == null) return "";

            return value.Value.ToString("d MMM", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date as a month-year string (e.g. Jan 2017)
        /// </summary>
        public static string ToMonthYearString(this DateTime? value)
        {
            if (value == null) return "";
            return value.Value.ToMonthYearString();
        }

        /// <summary>
        /// Returns the date as a month-year string (e.g. Jan 2017)
        /// </summary>
        public static string ToMonthYearString(this DateTime value)
        {
            return value.ToString("MMM yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date string format
        /// </summary>
        public static string ToDateString(this DateTime? value)
        {
            if (value == null) return "";

            return value.Value.ToDateString();
        }

        /// <summary>
        /// Returns the date in a compact year-month-date string format
        /// </summary>
        public static string ToDateStringCompact(this DateTime value)
        {
            return value.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a compact year-month-date string format
        /// </summary>
        public static string ToDateStringCompact(this DateTime? value)
        {
            if (value == null) return "";
            return value.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date and time string format
        /// </summary>
        public static string ToDateTimeString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        public static string ToFilenameDateTimeString(this DateTime value)
        {
            return value.ToString("yyMMdd-HHmm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date and time string format
        /// </summary>
        public static string ToDateTimeString(this DateTime? value)
        {
            if (value == null) return "";

            return ToDateTimeString((DateTime)value);
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date-hour-minute string format
        /// </summary>
        public static string ToDateTimeStringCompact(this DateTime value)
        {
            return value.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date and time string format with seconds included
        /// </summary>
        public static string ToDateTimeSecondString(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date and time string format with seconds included
        /// </summary>
        public static string ToDateTimeSecondStringCompact(this DateTime value)
        {
            return value.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable
        /// </summary>
        public static string ToTimeString(this DateTime value)
        {
            return value.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the date in a suitable year-month-date and time string format with seconds included
        /// </summary>
        public static string ToTimeSecondString(this DateTime value)
        {
            return value.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the UTC Now time in ISO8601 format. E.g. 2001-12-17T09:30:47Z. Also known as Zulu time.
        /// </summary>
        public static string UtcNowAsISO8601()
        {
            // See: http://stackoverflow.com/questions/114983/given-a-datetime-object-how-do-i-get-a-iso-8601-date-in-string-format

            return string.Concat(DateTime.UtcNow.ToString("s"), "Z");
        }

        /// <summary>
        /// Returns the datetime as a time-ago string, based on utc time
        /// </summary>
        public static string ToTimeAgo(this DateTime date)
        {
            var gap = DateTime.UtcNow - date;
            return $"{gap.Days}d {gap.Hours}h {gap.Minutes}m {gap.Seconds}s";
        }

        /// <summary>
        /// Adds business days to a date, ignores weekends as Saturday and Sunday. Does not consider public holidays.
        /// </summary>
        public static DateTime AddBusinessDays(this DateTime date, int businessDaysToAdd)
        {
            if (businessDaysToAdd < 0)
                throw new Exception("Adding negative business days is not supported");

            for (var i = 0; i < businessDaysToAdd; i++)
            {
                date = date.AddDays(1);
                if (date.DayOfWeek == DayOfWeek.Saturday)
                    date = date.AddDays(2);
                if (date.DayOfWeek == DayOfWeek.Sunday)
                    date = date.AddDays(1);
            }

            return date;
        }

        /// <summary>
        /// Returns the beginning of the month for the given date
        /// </summary>
        public static DateTime GetStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
        }

        public static Int32 CalculateAge(DateTime dateOfBirth)
        {
            DateTime temp = dateOfBirth;
            Int32 age = 0;
            while ((temp = temp.AddYears(1)) < DateTime.Now)
                age++;
            return age;
        }

        public static DateTime ToSouthAfrica(this DateTime dateTime)
        {
            return dateTime.AddHours(2);
        }


        /// <summary>
        /// Indicates if the given date is within the last x minutes (e.g. within the last half-hour if 30 provided).
        /// Assumes the date is UTC-based.
        /// </summary>
        public static bool IsWithinLastMinutes(this DateTime utcDate, int minutes)
        {
            return (DateTime.UtcNow - utcDate).TotalMinutes.ToIntCeiling() <= minutes;
        }

        public static string GetDaySuffix(this int day)
        {
            string suffix;

            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    suffix = "st";
                    break;
                case 2:
                case 22:
                    suffix = "nd";
                    break;
                case 3:
                case 23:
                    suffix = "rd";
                    break;
                default:
                    suffix = "th";
                    break;
            }

            return $"{day}{suffix}";
        }

        /// <summary>
        /// Gets the difference between two UTC dates in months.
        /// </summary>
        /// <param name="utcFrom"></param>
        /// <param name="utcTo"></param>
        /// <exception cref="ValidationException"></exception>
        /// <returns></returns>
        public static int GetDifferenceInMonths(this DateTime utcFrom, DateTime utcTo)
        {
            if (utcFrom.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Only UTC dates can be used", nameof(utcFrom));
            }

            if (utcTo.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Only UTC dates can be used.", nameof(utcTo));
            }

            if (utcFrom > utcTo) return GetDifferenceInMonths(utcTo, utcFrom);

            var monthDiff = Math.Abs((utcTo.Year * 12 + (utcTo.Month - 1)) - (utcFrom.Year * 12 + (utcFrom.Month - 1)));

            if (utcFrom.AddMonths(monthDiff) > utcTo || utcTo.Day < utcFrom.Day)
            {
                return monthDiff - 1;
            }

            return monthDiff;
        }


        public static int GetDifferenceInWeeksFromToday(DateTime from)
        {
            return GetDifferenceInWeeks(from, DateTime.Now);
        }

        public static int GetDifferenceInWeeks(this DateTime date, DateTime comparedTo, bool roundUp = false)
        {
            var ts = date.Subtract(comparedTo);
            var dateDiff = ts.Days;
            var result = Convert.ToDecimal(dateDiff / 7);

            return roundUp ? Math.Ceiling(result).ToInt() : Math.Floor(result).ToInt();
        }

        /// <summary>
        /// Indicates if the given date time is for midnight (has no time)
        /// </summary>
        public static bool IsMidnight(this DateTime dateTime)
        {
            return dateTime == dateTime.Date;
        }
    }
}
