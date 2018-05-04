using System;
using NodaTime;
namespace Smairo.Extensions
{
    public static class TimeExtensions
    {
        /// <summary>
        /// Converts time to specific timezone date
        /// </summary>
        /// <remarks>
        /// Unspecified kind will be translated to UTC as is
        /// </remarks>
        /// <param name="dateTime"></param>
        /// <param name="tz"></param>
        /// <returns>DateTime in Unspecified kind</returns>
        public static DateTime ToTimezoneFromUtc(this DateTime dateTime, string tz)
        {
            dateTime = EnsureTimeInUtc(dateTime);
            var timeZone = GetTimeZone(tz);
            var instant = Instant.FromDateTimeUtc(dateTime);
            return instant.InZone(timeZone).ToDateTimeUnspecified();
        }

        /// <summary>
        /// Return UTC time of given time from specific timezone
        /// </summary>
        /// <remarks>Does nothing to the time, if its kind is already in UTC</remarks>
        /// <param name="dateTime"></param>
        /// <param name="tz"></param>
        /// <returns>DateTime in UTC kind</returns>
        public static DateTime ToUtcFromTimezone(this DateTime dateTime, string tz)
        {
            if (dateTime.Kind == DateTimeKind.Utc) {
                return dateTime;
            }

            var localTime = LocalDateTime.FromDateTime(dateTime);
            var timeZone = GetTimeZone(tz);
            var zoned = localTime.InZoneStrictly(timeZone);
            return zoned
                .WithZone(DateTimeZone.Utc)
                .ToDateTimeUtc();
        }

        /// <summary>
        /// Get difference as int between UTC and specific timezone
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="tz"></param>
        /// <returns>hours from utc in int</returns>
        public static int GetOffsetInHours(this DateTime dateTime, string tz)
        {
            dateTime = EnsureTimeInUtc(dateTime);
            var timestampInstant = Instant.FromDateTimeUtc(dateTime);
            var timeZone = GetTimeZone(tz);
            var utcOffset = timeZone.GetUtcOffset(timestampInstant);
            return int.Parse(utcOffset.ToString());
        }

        /// <summary>
        /// Rounds up to next specific TimeSpan
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static DateTime RoundUp(this DateTime dateTime, TimeSpan timeSpan)
        {
            var remainderTicks = dateTime.Ticks % timeSpan.Ticks;
            var delta = remainderTicks != 0 ? timeSpan.Ticks - remainderTicks : 0;
            return new DateTime(dateTime.Ticks + delta, dateTime.Kind);
        }

        /// <summary>
        /// Rounds down to next specific TimeSpan
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static DateTime RoundDown(this DateTime dateTime, TimeSpan timeSpan)
        {
            var delta = dateTime.Ticks % timeSpan.Ticks;
            return new DateTime(dateTime.Ticks - delta, dateTime.Kind);

        }

        #region Private
        private static DateTimeZone GetTimeZone(string tz)
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tz);
            return timeZone ?? throw new InvalidOperationException("Invalid timezone");
        }

        private static DateTime EnsureTimeInUtc(DateTime dateTime)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Unspecified:
                    return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                case DateTimeKind.Local:
                case DateTimeKind.Utc:
                    return dateTime.ToUniversalTime();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

    }
}