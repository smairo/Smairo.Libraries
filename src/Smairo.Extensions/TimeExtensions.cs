using System;
using NodaTime;
namespace Smairo.Extensions
{
    public static class TimeExtensions
    {
        /// <summary>
        /// Return Local conversion of given date, using given timezone
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="tz"></param>
        /// <returns>DateTime in Unspecified kind</returns>
        public static DateTime ToLocalTime(this DateTime dateTime, string tz)
        {
            dateTime = ModifyToSameStampAsUtcKind(dateTime);
            var timeZone = GetTimezone(tz);
            var instant = Instant.FromDateTimeUtc(dateTime);
            return instant.InZone(timeZone).ToDateTimeUnspecified();
        }

        /// <summary>
        /// Return UTC conversion of given date, using given timezone
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timezone"></param>
        /// <returns>DateTime in UTC kind</returns>
        public static DateTime ToUtcTime(this DateTime dateTime, string timezone)
        {
            // Make sure that kind for stamp is UNSPECIFIED, even if given stamp has UTC kind
            dateTime = ModifyToSameStampAsUnspecifiedKind(dateTime);

            var localTime = LocalDateTime.FromDateTime(dateTime);
            var timeZone = GetTimezone(timezone);
            var zoned = localTime.InZoneStrictly(timeZone);

            // Return UTC conversion
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
            dateTime = ModifyToSameStampAsUtcKind(dateTime);
            var timestampInstant = Instant.FromDateTimeUtc(dateTime);
            var timeZone = GetTimezone(tz);
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
        /// <summary>
        /// Gets <see cref="DateTimeZone"/> or throw <see cref="InvalidOperationException"/> if given zone is invalid
        /// </summary>
        /// <param name="tz"></param>
        /// <returns></returns>
        private static DateTimeZone GetTimezone(string tz)
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tz);
            return timeZone ?? throw new InvalidOperationException("Invalid timezone");
        }

        /// <summary>
        /// Creates <see cref="DateTime"/> with exact same stamp, but in <see cref="DateTimeKind.Unspecified"/>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static DateTime ModifyToSameStampAsUnspecifiedKind(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Creates <see cref="DateTime"/> with exact same stamp, but in <see cref="DateTimeKind.Utc"/>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static DateTime ModifyToSameStampAsUtcKind(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Utc);
        }
        #endregion
    }
}