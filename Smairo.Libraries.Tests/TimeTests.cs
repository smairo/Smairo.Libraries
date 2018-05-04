﻿using System;
using System.Globalization;
using Smairo.Extensions;
using Xunit;

namespace Smairo.Libraries.Tests
{
    public class TimeTests
    {
        // Global variables
        private const string Tz = "Europe/Helsinki";
        private const string InvalidTz = "InvalidTimeZoneThatDoesNotExist";
        private DateTime _localTestable = new DateTime(2020, 1, 1, 2, 0, 0, DateTimeKind.Local);
        private DateTime _utcTestable = new DateTime(2020, 1, 1, 2, 0, 0, DateTimeKind.Utc);
        private DateTime _unspecifiedTestable = new DateTime(2020, 1, 1, 2, 0, 0, DateTimeKind.Unspecified);

        [Fact]
        public void Test_ToTimezoneFromUtc()
        {
            // "Europe/Helsinki" is two hours ahead in utc in winter
            var localActual = _localTestable.ToUniversalTime().AddHours(2);

            Assert.Equal(localActual, _localTestable.ToTimezoneFromUtc(Tz));
            Assert.Equal(_utcTestable.AddHours(2), _utcTestable.ToTimezoneFromUtc(Tz));
            Assert.Equal(_unspecifiedTestable.AddHours(2), _unspecifiedTestable.ToTimezoneFromUtc(Tz));
            Assert.Throws<InvalidOperationException>(() => _unspecifiedTestable.ToTimezoneFromUtc(InvalidTz));
        }

        [Fact]
        public void Test_ToUtcFromTimezone()
        {
            Assert.Equal(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), _localTestable.ToUtcFromTimezone(Tz));
            Assert.Equal(new DateTime(2020, 1, 1, 2, 0, 0, DateTimeKind.Utc), _utcTestable.ToUtcFromTimezone(Tz));
            Assert.Equal(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), _unspecifiedTestable.ToUtcFromTimezone(Tz));
            Assert.Throws<InvalidOperationException>(() => _unspecifiedTestable.ToUtcFromTimezone(InvalidTz));
        }

        [Fact]
        public void Test_GetOffsetInHours()
        {
            Assert.Equal(2, _localTestable.GetOffsetInHours(Tz));
            Assert.Equal(2, _utcTestable.GetOffsetInHours(Tz));
            Assert.Equal(2, _unspecifiedTestable.GetOffsetInHours(Tz));
            Assert.Throws<InvalidOperationException>(() => _unspecifiedTestable.ToUtcFromTimezone(InvalidTz));
        }

        [Theory]
        [InlineData("2017-01-01T00:07:00", "2017-01-01T00:09:00", 3)]
        [InlineData("2017-01-01T00:42:00", "2017-01-01T00:45:00", 15)]
        [InlineData("2017-01-01T00:31:00", "2017-01-01T01:00:00", 30)]
        [InlineData("2017-01-01T01:53:00", "2017-01-01T02:00:00", 60)]
        public void Test_RoundUp_Minutes(string test, string actual, int minutes)
        {
            var testable = DateTime.ParseExact(test, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var actually = DateTime.ParseExact(actual, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            Assert.Equal(actually, testable.RoundUp(TimeSpan.FromMinutes(minutes)));
        }

        [Theory]
        [InlineData("2017-01-01T00:00:01", "2017-01-02T00:00:00", 24)]
        [InlineData("2017-01-01T00:01:00", "2017-01-02T00:00:00", 24)]
        [InlineData("2017-01-01T01:00:00", "2017-01-02T00:00:00", 24)]
        public void Test_RoundUp_Hours(string test, string actual, int hours)
        {
            var testable = DateTime.ParseExact(test, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var actually = DateTime.ParseExact(actual, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            Assert.Equal(actually, testable.RoundUp(TimeSpan.FromHours(hours)));
        }

        [Theory]
        [InlineData("2017-01-01T00:07:00", "2017-01-01T00:06:00", 3)]
        [InlineData("2017-01-01T00:42:00", "2017-01-01T00:30:00", 15)]
        [InlineData("2017-01-01T00:29:00", "2017-01-01T00:00:00", 30)]
        [InlineData("2017-01-01T01:53:00", "2017-01-01T01:00:00", 60)]
        public void Test_RoundDown_Minutes(string test, string actual, int minutes)
        {
            var testable = DateTime.ParseExact(test, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var actually = DateTime.ParseExact(actual, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            Assert.Equal(actually, testable.RoundDown(TimeSpan.FromMinutes(minutes)));
        }
        [Theory]
        [InlineData("2017-01-01T00:00:01", "2017-01-01T00:00:00", 24)]
        [InlineData("2017-01-01T00:01:00", "2017-01-01T00:00:00", 24)]
        [InlineData("2017-01-01T01:00:00", "2017-01-01T00:00:00", 24)]
        public void Test_RoundDown_Hours(string test, string actual, int hours)
        {
            var testable = DateTime.ParseExact(test, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            var actually = DateTime.ParseExact(actual, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            Assert.Equal(actually, testable.RoundDown(TimeSpan.FromHours(hours)));
        }


    }
}